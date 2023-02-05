using System;
using Common.Interfaces;
using Common.Manager;
using Godot;
using Mdfry1.CustomResources;
using Mdfry1.Entities.Behaviors;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Entities.Components;
using Mdfry1.Entities.EnemyState;
using Mdfry1.Entities.Interfaces;
//using Mdfry1.Entities.Vision;
using Mdfry1.Logic.Sight;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;
using Microsoft.FSharp.Core;

namespace Mdfry1.Entities;

public class EnemyV4 : EnemyMovableBehavior, IEnemy
{
    private readonly StateMachine _stateMachine = new();

    [Export] private bool BloodSpatterEnabled { get; set; } 
    [Export] private bool SoundEnabled { get; set; } 


    [Export(PropertyHint.ResourceType, "Enemy1AudioResource")]
    public EntityAudioResource AudioResource { get; set; }

    public IPlayAudio SoundPlayer { get; set; }

    public BloodSpatter BloodSpatter { get; set; }
    public Position2D HitboxPivot { get; set; }

    [Export] public EnemyBehaviorStates DefaultState { get; set; } = EnemyBehaviorStates.Wander;

    private Label StateLabel { get; set; }

    public EnemyAnimationManager AnimationManager { get; set; }


    [Export] public NodePath PatrolPath { get; set; }

    public EnemyDataStore EnemyDataStore { get; set; }

    public IDamagableBehavior Damagable { get; private set; }

    public Node2D ObstacleAvoidance { get; set; }

    public Label Cooldown { get; set; }


    public void Init()
    {
        _stateMachine.AddState(new IdleEnemyState(this));
        _stateMachine.AddState(new PatrolEnemyState(this));
        _stateMachine.AddState(new ChaseEnemyStateV2(this));
        _stateMachine.AddState(new WanderState(this));

        if (!EnemyDataStore.LineOfSight) _stateMachine.TransitionTo(EnemyBehaviorStates.Patrol);

        if (EnemyDataStore.CurrentCoolDownCounter <= 0f) _stateMachine.TransitionTo(DefaultState);

        _stateMachine.TransitionTo(DefaultState);
    }

    public override void _Ready()
    {
        //InitAudioStreams();
        SoundPlayer = GetNode<SoundPlayer>("Behaviors/SoundPlayer");
        BloodSpatter = GetNode<BloodSpatter>("BloodSpatter");

        HitboxPivot = GetNode<Position2D>("HitboxPiviot");
        AnimationManager = GetNode<EnemyAnimationManager>("AnimationManager");
        AnimationManager.Sprite = GetNode<Sprite>("Sprite");
        StateLabel = GetNode<Label>("StateLabel");
        EnemyDataStore = GetNode<EnemyDataStore>("Status");
        EnemyDataStore.VisionManager = GetNode<RaycastVision>("Vision");
        OnMove += OnMoveHandler;

        ObstacleAvoidance = GetNode<Node2D>("ObstacleAvoidance");

        if (EnemyDataStore.VisionManager != null)
        {
            //var onTargetDetectionOption = new FSharpOption<Action<Node2D>>(OnTargetDetection);
            //var b = FSharpOption.Some<Action<Node2D>>(OnTargetDetection);
            //var b = FSharpOption.Some(onTargetDetectionOption);
            //var onTargetDetectionOption = new FSharpOption<FSharpFunc<Node2D, Unit>>(OnTargetDetection);
            //EnemyDataStore.VisionManager.OnTargetSeen
            EnemyDataStore.VisionManager.OnTargetSeen += OnTargetDetection;
            EnemyDataStore.VisionManager.OnTargetOutOfSight += OnTargetLost;
        }

        EnemyDataStore.Cooldown = GetNode<Label>("Cooldown");

        Cooldown = GetNode<Label>("Cooldown");
        EnemyDataStore.DebugLabel = GetNode<Label>("DebugLabel");
        Damagable = GetNode<DamagableBehavior>("Behaviors/Damagable");

        if (PatrolPath != null)
            EnemyDataStore.Init(PatrolPath);
        else
            _logger.Debug("this.PatrolPath is null");

        Damagable.Init(EnemyDataStore);
        Damagable.OnTakeDamage += OnTakeDamage;
        Damagable.EmptyHealthBarCallback += OnEmptyHealthBar;
        Damagable.HurtboxInvincibilityStartedCallback += OnHurtboxInvincibilityStarted;
        Damagable.HurtboxInvincibilityEndedCallback += OnHurtboxInvincibilityEnded;
        Init();
    }

    public override void _PhysicsProcess(float delta)
    {
        try
        {
            _stateMachine.Update(delta);
            if (IsDebugging) StateLabel.Text = _stateMachine.CurrentState.ToString();

            if (EnemyDataStore.CurrentCoolDownCounter > 0)
                EnemyDataStore.CurrentCoolDownCounter -= delta;
            else
                // We have lost our target and need to return to our default state. This could be more robust in the future. 
                _stateMachine.TransitionTo(DefaultState);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    private void InitAudioStreams()
    {
        // TakeDamageClipPlayer = GetNode<AudioStreamPlayer>("TakeDamageClipPlayer");
        // DeathClipPlayer= GetNode<AudioStreamPlayer>("DeathClipPlayer");
        // AttackClipPlayer = GetNode<AudioStreamPlayer>("AttackClipPlayer");
    }

    private void OnEmptyHealthBar()
    {
        if (SoundEnabled)
        {
            SoundPlayer.PlaySound(AudioResource.DeathClipPath);
        }

        _logger.Debug(Name + " Died");
        AnimationManager.PlayDeathAnimation();
        QueueFree();
    }

    private void OnHurtboxInvincibilityEnded()
    {
        _logger.Debug(Name + " OnHurtboxInvincibilityEnded");
    }

    private void OnHurtboxInvincibilityStarted()
    {
        _logger.Debug(Name + " OnHurtboxInvincibilityStarted");
    }

    private void OnTakeDamage(Node sender, Vector2 damageForce)
    {
        _logger.Debug(Name + " took damage");
        if (SoundEnabled)
        {
            SoundPlayer.PlaySound(AudioResource.TakeDamageClipPath);
        }

        AnimationManager.PlayTakeDamageAnimation();
        var bloodSpatter = (BloodSpatter)GD.Load<PackedScene>("res://Entities/Effects/BloodSpatter.tscn").Instance();
        if (BloodSpatterEnabled)
        {
            bloodSpatter.TargetGlobalPosition = GetTree().GetPlayerGlobalPosition();
            bloodSpatter.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(bloodSpatter);
        }
        MoveAndSlide(damageForce);
        Alert();
    }

    private void OnMoveHandler(Vector2 velocity, float delta)
    {
        AnimationManager?.UpdateAnimationBlendPositions(velocity);
        EnemyDataStore.VisionManager?.UpdateFacingDirection(velocity);

        if (_stateMachine.CurrentState.Name == "ChasePlayer")
            AnimationManager?.PlayRunAnimation();
        else
            AnimationManager?.PlayWalkAnimation();
    }

    private void OnTargetLost(Node2D target)
    {
        EnemyDataStore.CurrentCoolDownCounter = EnemyDataStore.MaxCoolDownTime;
    }

    private void OnTargetDetection(Node2D target)
    {
        Alert();
    }

    public void Alert()
    {
        EnemyDataStore.CurrentCoolDownCounter = EnemyDataStore.MaxCoolDownTime;
        _stateMachine.TransitionTo(EnemyBehaviorStates.ChasePlayer);
    }
}