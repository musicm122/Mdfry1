using System;
using Godot;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;
using Mdfry1.Entities.Values;
using Mdfry1.Scripts.Extensions;


namespace Mdfry1.Entities
{
    
    public class LampLight : Examinable
    {
        private const string LampFluidItem = "LampFluid";
        protected ILogger _logger { get; set; } = new GDLogger(LogLevelOutput.Debug);
        private Action<LightValue> OnLightLevelChanged { get; set; }
        private Action<LightValue, float> OnTimerStarted { get; set; }
        private Light2D Light { get; set; }
        
        public PlayerV2 PlayerV2 { get; set; }
        
        [Export()]
        public float LightLevelTransitionTime { get; set; } = 1f;
        
        public Timer Timer { get; set; }
        public AnimatedSprite AnimatedSprite { get; set; }
        
        private LightValue _lightValue = AvailableLightValues.High;
        
        private LightValue LightValue
        {
            get => _lightValue;
            set
            {
                _lightValue = value;
                OnLightLevelChanged?.Invoke(value);
            }
        }
        
        public override void _Ready()
        {
            base._Ready();
            AnimatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
            Light = GetNode<Light2D>("Light2D");
            Timer = GetNode<Timer>("Timer");
            this.Timer.Connect("timeout", this, nameof(OnTimerTimeout));
            (var hasPlayer, PlayerV2) = GetTree().GetPlayerNode();
            if (!hasPlayer)
            {
                _logger.Warning("LampLight: PlayerV2 not found");
            }
            
            Timer.WaitTime = LightLevelTransitionTime;
            StartTimer();
        }

        void StartTimer()
        {
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
        
        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            AnimatedSprite.Visible = (LightValue.Level != LightLevel.None);
            Light.Energy = LightValue.GetEnergyFluctuation();
        }

        protected override void OnInteract()
        {
            base.OnInteract();
            if (PlayerV2.DataStore.Inventory.HasItem(LampFluidItem) && LightValue.Level< LightLevel.High)
            {
                switch (LightValue.Level)
                {
                    case LightLevel.None:
                    case LightLevel.Low:
                    case LightLevel.Medium:
                        StartDialog(LampDialogInteractions.PromptForUsingFluid);
                        break;
                }
            }
            else if (LightValue.Level == LightLevel.High)
            {
                StartDialog(LampDialogInteractions.PromptForFluidFull);
            }
            else
            {
                StartDialog(LampDialogInteractions.PromptForNeedFluid);
            }
        }

        public override void OnDialogListener(string val)
        {
            base.OnDialogListener(val);
            _logger.Debug("OnDialogListener with val ", val);
            switch (val)
            {
                case "UsingLighterFluid":
                    AddFluid();
                    break;
            }
        }

        private void AddFluid()
        {
            PlayerV2.RemoveItems(LampFluidItem, 1);
            LightValue = AvailableLightValues.High;
            StartTimer();
        }
    }
}