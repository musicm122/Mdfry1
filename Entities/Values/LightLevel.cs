using System.ComponentModel;

namespace Mdfry1.Entities.Values
{

    public enum LightLevel
    {
        [Description("No Light")] None = 0,
        [Description("Low DayNightCycle")] Low,
        [Description("Moderate DayNightCycle")] Medium,
        [Description("Maximum DayNightCycle")] High
    }
}