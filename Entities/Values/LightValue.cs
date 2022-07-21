using System;
using Godot;
using Mdfry1.Scripts.Extensions;

namespace Mdfry1.Entities.Values
{
    public record LightValue(LightLevel Level, Color Color, float MinEnergy = 0f, float MaxEnergy = 0f)
    {
        public float GetEnergyFluctuation() => new Random().RandomFloat(MinEnergy, MaxEnergy);
    }
}