using Godot;

namespace Mdfry1.Entities.Values
{
    public static class AvailableLightValues
    {
        public static readonly LightValue High = 
            new LightValue(LightLevel.High, new Color(245f, 240f, 65f), 5f, 4.5f);

        public static readonly LightValue Medium =
            new LightValue(LightLevel.Medium, new Color(245f, 175f, 65f), 2f, 1.5f);

        public static readonly LightValue Low = 
            new LightValue(LightLevel.Low, new Color(155f, 125f, 10f), 0.5f, 0.2f);
        
        public static readonly LightValue None = 
            new LightValue(LightLevel.None, new Color(245f, 240f, 65f));

        public static LightValue[] Values { get; } = { High, Medium, Low, None };
    }
}