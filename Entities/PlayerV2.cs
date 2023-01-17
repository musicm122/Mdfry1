using Common.Manager;
using Godot;
using Mdfry1.CustomResources;
using Mdfry1.Entities.Behaviors;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Mission;

namespace Mdfry1.Entities;

public class PlayerV2 : PlayerMovableBehavior
{
    [Export(PropertyHint.ResourceType, "PlayerAudioResource")]
    public PlayerAudioResource AudioResource { get; set; }

    public PlayerDataStore DataStore { get; set; }
    
    [Export]
    public int MaxHealth
    {
        get => PlayerStatus.MaxHealth;
        set => PlayerStatus.MaxHealth = value;
    } 
    
    [Export]
    public int CurrentHealth
    {
        get => PlayerStatus.CurrentHealth;
        set => PlayerStatus.CurrentHealth = value;
    } 
    
    public IPlayAudio SoundPlayer { get; set; }

    public IDamagableBehavior Damagable { get; private set; }

    public IInteractableBehavior Interactable { get; private set; }

    public IUiBehavior Ui { get; private set; }

    public IFlashlightBehavior Flashlight { get; private set; }

    public IShootableBehavior ShootableBehavior { get; private set; }
    
    public Health PlayerStatus { get; set; }

    public PlayerAnimationManager AnimationManager { get; set; }

    public override void _Ready()
    {
        base._Ready();
        PlayerStatus = GetNode<Health>("Health");
        AnimationManager = GetNode<PlayerAnimationManager>("AnimationManager");
        
        DataStore = new PlayerDataStore
        {
            PlayerStatus = PlayerStatus,
            Inventory = new Inventory(),
            MissionManager = new MissionManager()
        };

        DataStore.Inventory.Add("Ammo", 1);
        DataStore.GetFacingDirection += AnimationManager.GetFacingDirection;

        SoundPlayer = GetNode<SoundPlayer>("Behaviors/SoundPlayer");

        Damagable = GetNode<DamagableBehavior>("Behaviors/Damagable");
        Damagable.Init(PlayerStatus);

        Interactable = GetNode<InteractableBehavior>("Behaviors/Interactable");
        Interactable.Init(DataStore);

        Flashlight = GetNode<FlashlightBehavior>("Behaviors/Flashlight");
        Flashlight.Init(DataStore);

        Ui = GetNode<UiBehavior>("UI");
        Ui.Init(DataStore);

        Interactable.InteractingCallback += e => CanMove = false;
        Interactable.InteractingCompleteCallback += e => CanMove = true;

        Damagable.OnTakeDamage += OnTakeDamage;
        Damagable.HurtboxInvincibilityStartedCallback += OnHurtboxInvincibilityStarted;
        Damagable.HurtboxInvincibilityEndedCallback += OnHurtboxInvincibilityEnded;

        OnPhysicsProcessMovement += OnProcessMovement;
        OnMove += OnWalkAction;
        OnIdle += OnIdleAction;
        OnRoll += OnRollAction;

        ShootableBehavior = GetNode<IShootableBehavior>("Behaviors/Shootable");
        ShootableBehavior.Init(DataStore);
        ShootableBehavior.OnShootStart += OnShootStarted;
        ShootableBehavior.OnNoAmmo += OnShootStartedWithEmptyClip;
        PlayerStatus.EmptyHealthBarCallback += OnDeath;
    }

    private void OnDeath()
    {
        SoundPlayer.PlaySound(AudioResource.DeathClipPath);
    }

    private void OnShootStarted()
    {
        SoundPlayer.PlaySound(AudioResource.AttackClipPath);
        AnimationManager.PlayShootAnimation(Velocity);
        DataStore.DecrementAmmo();
        Ui.RefreshUI();
    }

    private void OnShootStartedWithEmptyClip()
    {
        SoundPlayer.PlaySound(AudioResource.EmptyClipPath);
        AnimationManager.PlayEmptyClipAnimation(Velocity);
    }

    private void OnHurtboxInvincibilityEnded()
    {
        AnimationManager?.StopBlinkAnimation();
    }

    private void OnHurtboxInvincibilityStarted()
    {
        AnimationManager?.StartBlinkAnimation();
    }

    private void OnProcessMovement(Vector2 vector2)
    {
        AnimationManager?.UpdateAnimationBlendPositions(vector2);
    }

    private void OnRollAction(Vector2 velocity)
    {
        SoundPlayer.PlaySound(AudioResource.DashClipPath);
        AnimationManager?.PlayRollAnimation(velocity);
    }

    private void OnIdleAction(Vector2 velocity, float delta)
    {
        AnimationManager?.PlayIdleAnimation(velocity);
    }

    private void OnWalkAction(Vector2 velocity, float delta)
    {
        AnimationManager?.PlayWalkAnimation(velocity);
    }

    private void OnTakeDamage(Node sender, Vector2 damageForce)
    {
        SoundPlayer.PlaySound(AudioResource.TakeDamageClipPath);
        MoveAndSlide(damageForce);
        Ui.RefreshUI();
    }

    public void AddItem(string name, int amt)
    {
        Ui.AddItem(name, amt);
    }

    public void RemoveItems(string name, int amt)
    {
        Ui.RemoveItems(name, amt);
    }

    public void AddMission(string title)
    {
        Ui.AddMission(title);
    }
}