using Godot;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;

namespace Mdfry1.Scripts.Constants;

public static class MasterItemList
{
    public static readonly Item.Item[] Items =
    {
        new("Foo's Glasses", "These are some thick lenses",
            ItemConstants.FlashlightImagePath),
        new("Flashlight", "Essential for seeing in the dark.",
            ItemConstants.FlashlightImagePath),
        new("Gas Mask", "Allows me to survive tear gas",
            ItemConstants.FlashlightImagePath),
        new("Broken cell phone", "Maybe it can be fixed",
            ItemConstants.FlashlightImagePath),
        new("Carton of Milk", "For use with treating pepper spray",
            ItemConstants.FlashlightImagePath),
        new("Survival Manual", "Basics on surviving.",
            ItemConstants.FlashlightImagePath),
        new("HealthKit", "Heals damage", ItemConstants.FlashlightImagePath),
        new("Ammo", "Ammo for your gun", ItemConstants.PizzaImagePath),
        new("KeyA", "Opens 'A' doors", ItemConstants.KeyImagePath),
        new("LampFluid", "Keeps the fire lit",
            ItemConstants.LampFluidImagePath)
    };

    public static Item.Item GetItemByName(string name)
    {
        var result = Items.Find(i => i.Name == name);
        if (result != null) return result;
        GD.PrintErr($"GetItemByName({name}) not found");
        throw new ItemNotFoundException(name);
    }
}