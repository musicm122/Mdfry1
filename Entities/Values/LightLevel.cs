using System.ComponentModel;

namespace Mdfry1.Entities.Values
{

    public enum LightLevel
    {
        [Description("No Light")] None = 0,
        [Description("Low Lighting")] Low,
        [Description("Moderate Lighting")] Medium,
        [Description("Maximum Lighting")] High
    }
}