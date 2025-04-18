using Core.Input;
using Godot;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Mission;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities;

public class PauseMenu : Control, IDebuggable<Node>
{
    [Export] private readonly float PauseToggleCooldownWaitTime = 1.0f;

    private float AccumulatorPauseToggleCooldown;
    private bool CanTogglePause = true;

    [Export] public bool IsPauseOptionEnabled { get; set; } = true;

    [Export] private string InventoryDisplayPath { get; set; } = "./InventoryPanel/InventoryDisplay";

    [Export] private string MissionManagerDisplayPath { get; set; } = "./MissionPanel/MissionDisplay";

    [Export] private string TitleDisplayPath { get; set; } = "./TitlePanel/Title";


    public Label InventoryDisplay { get; set; }
    public Label MissionDisplay { get; set; }
    public Label TitleDisplay { get; set; }

    public bool IsHidden { get; set; }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public override void _Ready()
    {
        TitleDisplay = GetNode<Label>(TitleDisplayPath);
        InventoryDisplay = GetNode<Label>(InventoryDisplayPath);
        MissionDisplay = GetNode<Label>(MissionManagerDisplayPath);

        if (TitleDisplay == null) this.Print("TitleDisplay is null");
        if (InventoryDisplay == null) this.Print("InventoryDisplay is null");
        if (MissionDisplay == null) this.Print("MissionDisplay is null");
    }

    public override void _Process(float delta)
    {
        if (!IsPauseOptionEnabled) return;
        if (Input.IsActionPressed(InputConstants.Pause))
        {
            if (CanTogglePause)
            {
                this.Print("Can toggle pause yet, time left ");
                CanTogglePause = false;
                TogglePauseMenu();
                AccumulatorPauseToggleCooldown = PauseToggleCooldownWaitTime;
            }
            else
            {
                this.Print("Cannot toggle pause yet, time left ", AccumulatorPauseToggleCooldown);
            }
        }

        if (AccumulatorPauseToggleCooldown > 0)
            AccumulatorPauseToggleCooldown -= delta;
        else
            CanTogglePause = true;
    }

    private void TogglePauseMenu()
    {
        this.Print("Toggling Pause Menu");
        this.TogglePause();
        if (this.IsPaused())
        {
            this.Print("Should be showing Pause Menu");
            Show();
        }
        else
        {
            this.Print("Should be hiding Pause Menu");
            Hide();
        }
    }

    public void RefreshUI(Inventory inventory, MissionManager missionManager)
    {
        InventoryDisplay.Text = inventory.Display();
        MissionDisplay.Text = missionManager.Display();
    }
}