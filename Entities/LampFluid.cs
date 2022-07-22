using Godot;
using Mdfry1.Entities.Values;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;

namespace Mdfry1.Entities
{

    public class LampFluid : Examinable
    {
        protected override void OnInteract()
        {
            StartDialog(LampDialogInteractions.LampFluidFound);
            base.OnInteract();
            
        }

        public override void OnDialogListener(string val)
        {
            const string item = "LampFluid";
            const string expectedDialogArg = "LampFluidFound";
            base.OnDialogListener(val);
            if (val != expectedDialogArg) return;
            GetTree().AddItem(item);
            ShouldRemove = true;
        }
    }
}