using System.Collections.Generic;

namespace Mdfry1.Scripts.Constants;

public static class ItemConstants
{
    public const string PizzaImagePath = "res://Assets/Art/Food/pizzaslice.png";
    public const string FlashlightImagePath = "res://Assets/Art/ItemDefinition/Flashlight.png";
    public const string HealthkitImagePath = "res://Assets/Art/ItemDefinition/genericItem_color_102.png";
    public const string KeyImagePath = "res://Assets/Art/ItemDefinition/genericItem_color_155.png";
    public const string LampFluidImagePath = "res://Assets/Art/ItemDefinition/genericItem_color_107.png";


    public static readonly Dictionary<string, string> ItemImagePaths = new()
    {
        ["Pizza"] = PizzaImagePath,
        ["Flashlight"] = FlashlightImagePath,
        ["KeyA"] = KeyImagePath,
        ["Healthkit"] = HealthkitImagePath,
        ["LampFluid"] = LampFluidImagePath
    };
}