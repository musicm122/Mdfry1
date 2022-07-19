using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;

namespace Mdfry1.Scripts.Constants
{
    public static class MasterItemList
    {

        public static readonly Item.Item[] Items = {
        new Item.Item(){
            Name = "Foo's Glasses",
            Description= "These are some thick lenses",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "Flashlight",
            Description= "Essential for seeing in the dark.",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "Gas Mask",
            Description= "Allows me to survive tear gas",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "Broken cell phone",
            Description= "Maybe it can be fixed",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "Carton of Milk",
            Description= "For use with treating pepper spray",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "Survival Manual",
            Description= "Basics on surviving.",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "HealthKit",
            Description= "Heals damage",
            ImagePath = ItemConstants.FlashlightImagePath},
        new Item.Item(){
            Name = "Ammo",
            Description= "Ammo for your gun",
            ImagePath = ItemConstants.PizzaImagePath},
        new Item.Item(){
            Name = "KeyA",
            Description= "Opens 'A' doors",
            ImagePath = ItemConstants.KeyImagePath},
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
