using Godot;

namespace Mdfry1.Entities.Values;

public static class LightColor
{
    public static Color Low { get; set; } = new(155f, 125f, 10f);
    public static Color Medium { get; set; } = new(245f, 175f, 65f);
    public static Color High { get; set; } = new(245f, 240f, 65f);
}