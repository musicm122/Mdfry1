using Godot;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities;

public class PushBlock : KinematicBody2D, IDebuggable<Node>
{
    [Export] public bool CanBePushed { get; set; } = true;

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public void Push(Vector2 velocity)
    {
        MoveAndSlide(velocity);
    }
}