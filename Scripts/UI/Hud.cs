using Godot;
using Mdfry1.Entities;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Scripts.UI;

public class Hud : Control, IDebuggable<Node>
{
    private string HealthBarDisplayPath { get; } = "./HealthPanel/HealthBar";
    private string GameOverPanelPath { get; } = "./GameOverPanel";

    private string DeathTextPath { get; } = "./GameOverPanel/DeathText";


    private string AmmoDisplayPath { get; } = "./AmmoPanel/Ammo";
    public Label AmmoDisplay { get; set; }

    public Label DeathText { get; set; }

    public Label HealthBarDisplay { get; set; }

    public PanelContainer GameOverPanel { get; set; }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

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
        var maxHealthVal = "Health: ";
        for (var i = 0; i < dataStore.PlayerStatus.MaxHealth; i++)
            if (i < dataStore.PlayerStatus.CurrentHealth)
                maxHealthVal += "|X|";
            else
                maxHealthVal += "| |";
        HealthBarDisplay.Text = $"{maxHealthVal}";
        AmmoDisplay.Text = $"Ammo: {dataStore.GetAmmoCount().ToString()}";
        if (dataStore.PlayerStatus.CurrentHealth <= 0)
        {
            DeathText.Text = $"You Died!\r\n You survived for {GetTree().GetDayCount().ToString()} Days!";
            GameOverPanel.Show();
        }
    }
}