using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;

namespace Mdfry1.Scripts.Constants
{
    public static class MasterItemList
    {
        public static readonly Item.Item[] Items =
        {
            new Item.Item(name: "Foo's Glasses", description: "These are some thick lenses",
                imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "Flashlight", description: "Essential for seeing in the dark.",
                imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "Gas Mask", description: "Allows me to survive tear gas",
                imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "Broken cell phone", description: "Maybe it can be fixed",
                imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "Carton of Milk", description: "For use with treating pepper spray",
                imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "Survival Manual", description: "Basics on surviving.",
                imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "HealthKit", description: "Heals damage", imagePath: ItemConstants.FlashlightImagePath),
            new Item.Item(name: "Ammo", description: "Ammo for your gun", imagePath: ItemConstants.PizzaImagePath),
            new Item.Item(name: "KeyA", description: "Opens 'A' doors", imagePath: ItemConstants.KeyImagePath),
            new Item.Item(name: "LampFluid", description: "Keeps the fire lit",
                imagePath: ItemConstants.LampFluidImagePath),
        };

        public static Item.Item GetItemByName(string name)
        {
            var result = Items.Find(i => i.Name == name);
            if (result != null) return result;
            GD.PrintErr($"GetItemByName({name}) not found");
            throw new ItemNotFoundException(name);
        }
    }
}