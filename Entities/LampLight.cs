using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Mdfry1.addons.dialogic.Other;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;
using Mdfry1.Entities.Values;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.GDUtils;


namespace Mdfry1.Entities
{
    public class LampLight : Examinable
    {
        public void EnableSpawning()
        {
            Spawner.EnableSpawning = true;
        }
        
        public void DisableSpawning()
        {
            Spawner.EnableSpawning = true;
        }
        
        public bool CanFlicker(int interval = 2) => new Random().Next() % interval == 0;
        
        [Export]
        public LightLevel DefaultLightLevel { get; set; } = LightLevel.Low;
        
        [Export]
        public bool IsFixedLightLevel = false;
        
        [Export()]
        public Dictionary<LightLevel, float> SpawnRates { get; set; } = 
            new()
            {
                {LightLevel.None, 1},
                {LightLevel.Low, 3f},
                {LightLevel.Medium, 5f},
                {LightLevel.High, 20f}
            };
        
        private const string LampFluidItem = "LampFluid";
        protected ILogger _logger { get; set; } = new GDLogger(LogLevelOutput.Debug);
        public Action<LightValue> OnLightLevelChanged { get; set; }
        public Action<LightValue, float> OnTimerStarted { get; set; }
        
        private Light2D Light { get; set; }
        
        public EnemySpawner Spawner { get; set; }
        
        public PlayerV2 PlayerV2 { get; set; }
        
        [Export()]
        public float LightLevelTransitionTime { get; set; } = 1f;
        
        public Timer Timer { get; set; }
        public AnimatedSprite AnimatedSprite { get; set; }
        
        private LightValue _lightValue = AvailableLightValues.High;
        
        public LightValue LightValue
        {
            get => _lightValue;
            set
            {
                _lightValue = value;
                _logger.Debug("LightValue changed to " + value);
                Spawner.SpawnRate = SpawnRates[_lightValue.Level];
                Spawner.EnemySpawnState = _lightValue.Level == LightLevel.None ? EnemyBehaviorStates.ChasePlayer : EnemyBehaviorStates.Wander;
                OnLightLevelChanged?.Invoke(value);
            }
        }
        
        public override void _Ready()
        {
            InteractableArea = this.GetNode<Area2D>("Area2D");
            AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            Spawner = GetNode<EnemySpawner>("EnemySpawner");
            Spawner.SpawnRate = SpawnRates[_lightValue.Level];
            Light = GetNode<Light2D>("Light2D");
            LightValue = AvailableLightValues.Values.Find(v=>v.Level == DefaultLightLevel);
            
            if(!IsFixedLightLevel)
            {
                Timer = GetNode<Timer>("Timer");
                this.Timer.Connect("timeout", this, nameof(OnTimerTimeout));
            }
            
            if (InteractableArea != null)
            {
                RegisterInteractable(InteractableArea);
            }
            else
            {
                throw new NullReferenceException("Missing required child 'Area2D' for examinable");
            }
            (var hasPlayer, PlayerV2) = GetTree().GetPlayerNode();
            if (!hasPlayer)
            {
                _logger.Warning("LampLight: PlayerV2 not found");
            }
            StartTimer();
        }

        void StartTimer()
        {
            Timer.WaitTime = LightLevelTransitionTime;
            Timer.Start();
            _logger.Debug("StartTimer with Light Level ", LightValue.Level.GetDescription());
            _logger.Debug("Starting timer");
            OnTimerStarted?.Invoke(LightValue, Timer.WaitTime);
        }

        void OnTimerTimeout()
        {
            this.LightValue = DecrementLightLevel(this.LightValue);
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
            
            if (CanInteract && InputUtils.IsInteracting())
            {
                OnInteract();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            AnimatedSprite.Visible = (LightValue.Level != LightLevel.None);
            if (CanFlicker(20))
            {
                Light.Energy = LightValue.GetEnergyFluctuation();
            }
        }

        protected override void OnInteract()
        {
            if (PlayerV2.DataStore.Inventory.HasItem(LampFluidItem) && LightValue.Level< LightLevel.High)
            {
                switch (LightValue.Level)
                {
                    case LightLevel.None:
                    case LightLevel.Low:
                    case LightLevel.Medium:
                        this.Timeline = LampDialogInteractions.PromptForUsingFluid;
                        break;
                }
            }
            else if (LightValue.Level == LightLevel.High)
            {
                this.Timeline = LampDialogInteractions.PromptForFluidFull;
            }
            else
            {
                this.Timeline = LampDialogInteractions.PromptForNeedFluid;
            }
            StartDialog(this.Timeline);
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
            {
                AddChild(dialog);
            }
            else
            {
                this._logger.Error($"{nameof(Examinable)} StartDialog(${timeLine}) failed with error {result}");
            }
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
            if (body.IsPlayer() && this.HasSignal(nameof(PlayerInteractingAvailable)))
            {
                EmitSignal(nameof(PlayerInteractingAvailable), this);
            }
        }
        
        private async Task DialogComplete()
        {
            this._logger.Debug($"Examinable.{nameof(DialogComplete)} called");
            this._logger.Debug($"Examinable.ShouldRemove = {ShouldRemove}");
            EmitSignal(nameof(PlayerInteractingComplete), this);
            await this.WaitForSeconds(0.2f).ConfigureAwait(false);
            this.Unpause();
            CanInteract = true;

            if (ShouldRemove)
            {
                RemoveItem();
            }
        }
        
        private void RemoveItem()
        {
            this.Unpause();
            if (this.HasSignal(nameof(PlayerInteractingUnavailable)))
            {
                EmitSignal(nameof(PlayerInteractingAvailable), this);
            }
            Visible = false;
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);

            if (GetChildCount() > 0 && InteractableArea != null)
            {
                RemoveChild(InteractableArea);
            }
        }

        public override  void OnExaminableAreaExited(Node body)
        {
            if (body.IsPlayer() && this.HasSignal(nameof(PlayerInteractingUnavailable)))
            {
                EmitSignal(nameof(PlayerInteractingUnavailable), this);
            }
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
            area2D.ConnectBodyEntered(this, nameof(OnExaminableAreaEntered));
            area2D.ConnectBodyExited(this, nameof(OnExaminableAreaExited));
        }
    }
}