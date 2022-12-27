using Godot;

namespace Mdfry1.Entities.Components;

public class Target : Node2D
{
    public Vector2 GetAimAtPoint()
    {
        return (GlobalTransform.origin + Vector2.Up) * 1.5f;
    }
}