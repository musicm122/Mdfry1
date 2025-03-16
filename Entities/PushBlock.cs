using Godot;
using Mdfry1.Logic.Constants;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities;

public class PushBlock : KinematicBody2D, IDebuggable<Node>
{
    [Export] public bool CanBePushed { get; set; } = true;

    [Export] public bool IsDebugging { get; set; }

    public override void _Ready()
    {
        AddToGroup(Groups.Pushable);
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup(Groups.Pushable);

    }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public void Push(Vector2 velocity)
    {
        MoveAndSlide(velocity);
    }
}