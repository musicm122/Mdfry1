using Godot;
using Mdfry1.Entities;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Scripts.UI
{
    public class Hud : Control, IDebuggable<Node>
    {
        [Export]
        public bool IsDebugging { get; set; } = false;

        private string HealthBarDisplayPath { get; set; } = "./HealthPanel/HealthBar";
        private string GameOverPanelPath { get; set; } = "./GameOverPanel";
        
        private string DeathTextPath { get; set; } = "./GameOverPanel/DeathText";
        
        
        private string AmmoDisplayPath { get; set; } = "./AmmoPanel/Ammo";
        public Label AmmoDisplay { get; set; }
        
        public Label DeathText { get; set; }

        public Label HealthBarDisplay { get; set; }

        public PanelContainer GameOverPanel { get; set; }

        public bool IsDebugPrintEnabled() => IsDebugging;

        public override void _Ready()
        {
            HealthBarDisplay = GetNode<Label>(HealthBarDisplayPath);
            GameOverPanel = GetNode<PanelContainer>(GameOverPanelPath);
            DeathText = GetNode<Label>(DeathTextPath);
            AmmoDisplay = GetNode<Label>(AmmoDisplayPath);
            GameOverPanel.Hide();
        }

        public void RefreshUI(PlayerDataStore dataStore)
        {
            string maxHealthVal = "Health: ";
            for (int i = 0; i < dataStore.PlayerStatus.MaxHealth; i++)
            {
                if (i < dataStore.PlayerStatus.CurrentHealth)
                {
                    maxHealthVal += "|X|";
                }
                else
                {
                    maxHealthVal += "| |";
                }
            }
            HealthBarDisplay.Text = $"{maxHealthVal}";
            AmmoDisplay.Text = $"Ammo: {dataStore.GetAmmoCount().ToString()}";
            if (dataStore.PlayerStatus.CurrentHealth <= 0)
            {
                DeathText.Text = $"You Died!\r\n You survived for {GetTree().GetDayCount().ToString()} Days!"; 
                GameOverPanel.Show();
            }
        }
    }
}