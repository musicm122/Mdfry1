using System;
using System.Threading.Tasks;
using Core.Input;
using Godot;
using Godot.Collections;
using Mdfry1.addons.dialogic.Other;
using Mdfry1.Entities.Components;
using Mdfry1.Entities.Values;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities;

public class LampLight : Examinable
{
    private const string LampFluidItem = "LampFluid";

    private LightValue _lightValue = AvailableLightValues.High;

    [Export] public bool IsFixedLightLevel;

    [Export] public bool SpawnerEnabled { get; set; } = true;

    [Export]
    public int MaxSpawnCount
    {
        get => Spawner.MaxSpawnCount;
        set => Spawner.MaxSpawnCount = value;
    }

    [Export(PropertyHint.File, "*.tscn")]
    public string EnemyToSpawnPath
    {
        get => Spawner != null ? Spawner.EnemyToSpawnPath : string.Empty;
        set {
            if (Spawner?.EnemyToSpawnPath != null)
            {
                Spawner.EnemyToSpawnPath = value;
            }
        }
    }
    
    // [Export(PropertyHint.File, "*.tscn")]
    // public string EnemyToSpawnPath
    // {
    //     get =>
    //         Spawner != null ? Spawner.EnemyToSpawnPath : string.Empty;
    //         
    //     set => 
    //
    //         if (Spawner != null) {
    //             Spawner.EnemyToSpawnPath = value;
    //         }else{
    //             Spawner.EnemyToSpawnPath = string.Empty;
    //         }
    // }
    //


    [Export] public LightLevel DefaultLightLevel { get; set; } = LightLevel.Low;

    [Export]
    public Dictionary<LightLevel, float> SpawnRates { get; set; } =
        new()
        {
            { LightLevel.None, 1 },
            { LightLevel.Low, 3f },
            { LightLevel.Medium, 5f },
            { LightLevel.High, 20f }
        };

    protected ILogger _logger { get; set; } = new GDLogger(LogLevelOutput.Debug);
    public Action<LightValue> OnLightLevelChanged { get; set; }
    public Action<LightValue, float> OnTimerStarted { get; set; }

    private Light2D Light { get; set; }

    public EnemySpawner Spawner { get; set; }

    public PlayerV2 PlayerV2 { get; set; }

    [Export] public float LightLevelTransitionTime { get; set; } = 1f;

    public Timer Timer { get; set; }
    public AnimatedSprite AnimatedSprite { get; set; }

    public LightValue LightValue
    {
        get => _lightValue;
        set
        {
            _lightValue = value;
            _logger.Debug("LightValue changed to " + value);
            if (SpawnerEnabled)
            {
                Spawner.SpawnRate = SpawnRates[_lightValue.Level];
                Spawner.EnemySpawnState = EnemyBehaviorStates.ChasePlayer;
                //Spawner.EnemySpawnState = _lightValue.Level == LightLevel.None ? EnemyBehaviorStates.ChasePlayer : EnemyBehaviorStates.Wander;
            }

            OnLightLevelChanged?.Invoke(value);
        }
    }

    public void EnableSpawning()
    {
        Spawner.EnableSpawning = true;
    }

    public void DisableSpawning()
    {
        Spawner.EnableSpawning = false;
    }

    public bool CanFlicker(int interval = 2)
    {
        return new Random().Next() % interval == 0;
    }

    public override void _Ready()
    {
        InteractableArea = GetNode<Area2D>("Area2D");
        AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        Spawner = GetNode<EnemySpawner>("EnemySpawner");
        Spawner.EnemyToSpawnPath = this.EnemyToSpawnPath;
        Spawner.SpawnRate = SpawnRates[_lightValue.Level];
        Light = GetNode<Light2D>("Light2D");
        LightValue = AvailableLightValues.Values.Find(v => v.Level == DefaultLightLevel);
        Timer = GetNode<Timer>("Timer");

        if (!IsFixedLightLevel)
        {
            Timer.Connect("timeout", this, nameof(OnTimerTimeout));
        }

        if (InteractableArea != null)
            RegisterInteractable(InteractableArea);
        else
            throw new NullReferenceException("Missing required child 'Area2D' for examinable");
        (var hasPlayer, PlayerV2) = GetTree().GetPlayerNode();
        if (!hasPlayer) _logger.Warning("LampLight: PlayerV2 not found");
        StartTimer();
    }

    private void StartTimer()
    {
        Timer.WaitTime = LightLevelTransitionTime;
        Timer.Start();
        _logger.Debug("StartTimer with Light Level ", LightValue.Level.GetDescription());
        _logger.Debug("Starting timer");
        OnTimerStarted?.Invoke(LightValue, Timer.WaitTime);
    }

    private void OnTimerTimeout()
    {
        LightValue = DecrementLightLevel(LightValue);
    }

    private static LightValue DecrementLightLevel(LightValue lightValue)
    {
        return lightValue.Level switch
        {
            LightLevel.Low => AvailableLightValues.None,
            LightLevel.Medium => AvailableLightValues.Low,
            LightLevel.High => AvailableLightValues.Medium,
            _ => AvailableLightValues.None
        };
    }

    public override void _Process(float delta)
    {
        if (CanInteract && PlayerActions.isInteracting()) OnInteract();
    }

    public override void _PhysicsProcess(float delta)
    {
        try
        {
            AnimatedSprite.Visible = LightValue.Level != LightLevel.None;
            if (CanFlicker(20)) Light.Energy = LightValue.GetEnergyFluctuation();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected override void OnInteract()
    {
        if (PlayerV2.DataStore.Inventory.HasItem(LampFluidItem) && LightValue.Level < LightLevel.High)
            switch (LightValue.Level)
            {
                case LightLevel.None:
                case LightLevel.Low:
                case LightLevel.Medium:
                    Timeline = LampDialogInteractions.PromptForUsingFluid;
                    break;
            }
        else if (LightValue.Level == LightLevel.High)
            Timeline = LampDialogInteractions.PromptForFluidFull;
        else
            Timeline = LampDialogInteractions.PromptForNeedFluid;

        StartDialog(Timeline);
        //base.OnInteract();
    }

    public override void OnDialogListener(string val)
    {
        _logger.Debug("OnDialogListener with val ", val);
        switch (val)
        {
            case "UsingLampFluid":
                AddFluidToLamp();
                break;
        }
    }

    public override void StartDialog(string timeLine)
    {
        EmitSignal(nameof(PlayerInteracting), this);
        CanInteract = false;
        var dialog = DialogicSharp.Start(timeLine);
        var result = dialog.Connect("dialogic_signal", this, nameof(DialogListener));
        if (result == Error.Ok)
            AddChild(dialog);
        else
            _logger.Error($"{nameof(Examinable)} StartDialog(${timeLine}) failed with error {result}");
    }

    public override void DialogListener(object value)
    {
        this.Pause();
        var val = value.ToString();
        OnDialogListener(val);
        Task.Run(async () => await DialogComplete().ConfigureAwait(false));
    }

    public override void OnExaminableAreaEntered(Node body)
    {
        if (body.IsPlayer() && HasSignal(nameof(PlayerInteractingAvailable)))
            EmitSignal(nameof(PlayerInteractingAvailable), this);
    }

    private async Task DialogComplete()
    {
        _logger.Debug($"Examinable.{nameof(DialogComplete)} called");
        _logger.Debug($"Examinable.ShouldRemove = {ShouldRemove}");
        EmitSignal(nameof(PlayerInteractingComplete), this);
        await this.WaitForSeconds(0.2f).ConfigureAwait(false);
        this.Unpause();
        CanInteract = true;

        if (ShouldRemove) RemoveItem();
    }

    private void RemoveItem()
    {
        this.Unpause();
        if (HasSignal(nameof(PlayerInteractingUnavailable))) EmitSignal(nameof(PlayerInteractingAvailable), this);
        Visible = false;
        SetProcess(false);
        SetPhysicsProcess(false);
        SetProcessInput(false);

        if (GetChildCount() > 0 && InteractableArea != null) RemoveChild(InteractableArea);
    }

    public override void OnExaminableAreaExited(Node body)
    {
        if (body.IsPlayer() && HasSignal(nameof(PlayerInteractingUnavailable)))
            EmitSignal(nameof(PlayerInteractingUnavailable), this);
    }

    private void AddFluidToLamp()
    {
        PlayerV2.DataStore.Inventory.Remove(LampFluidItem);
        //PlayerV2.RemoveItems(LampFluidItem, 1);
        LightValue = AvailableLightValues.High;
        StartTimer();
    }

    private void RegisterInteractable(Area2D area2D)
    {
        area2D.TryConnectSignal(Signals.Area2D.BodyEntered, this, nameof(OnExaminableAreaEntered));
        area2D.TryConnectSignal(Signals.Area2D.BodyExited, this, nameof(OnExaminableAreaExited));
    }
}