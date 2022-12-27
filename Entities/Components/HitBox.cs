using Godot;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Components;

public class HitBox : Area2D, IDebuggable<Node>, IHitBox
{
    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    [Export] public int Damage { get; set; } = 1;

    [Export] public float EffectForce { get; set; } = 50f;
}