using Godot;

namespace Mdfry1.Entities.Values;

public static class AvailableLightValues
{
    public static readonly LightValue High = new(LightLevel.High, new Color(245f, 240f, 65f), 1f, 1.5f);

    public static readonly LightValue Medium = new(LightLevel.Medium, new Color(245f, 175f, 65f), 0.8f, 1f);

    public static readonly LightValue Low = new(LightLevel.Low, new Color(155f, 125f, 10f), 0.3f, 0.5f);

    public static readonly LightValue None = new(LightLevel.None, new Color(245f, 240f, 65f), 0.2f, 0.3f);

    public static LightValue[] Values { get; } = { High, Medium, Low, None };
}