using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.Constants;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Behaviors
{
    public class FlashlightBehavior : Node2D, IDebuggable<Node>, IFlashlightBehavior
    {
        public PlayerDataStore DataStore { get; set; }

        public void Init(PlayerDataStore dataStore)
        {
            DataStore = dataStore;
        }

        [Export]
        public bool IsDebugging { get; set; }

        public bool IsDebugPrintEnabled() => IsDebugging;

        public bool HasFlashlight =>
            DataStore.Inventory.HasItemInInventory("Flashlight");

        public Light2D Flashlight { get; set; }
        private void ToggleFlashlight()
        {
            this.PrintCaller();
            Flashlight.Enabled = !Flashlight.Enabled;
        }

        public override void _Ready()
        {
            Flashlight = GetNode<Light2D>("Flashlight");
            Flashlight.Enabled = false;
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionJustPressed(InputAction.ToggleFlashlight) && HasFlashlight)
            {
                ToggleFlashlight();
            }
        }

    }
}