using Godot;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Scripts.Item;

public class ItemBehavior : Area2D, IDebuggable<Node>
{
    public string Direction = "down";

    public string Property = "scale";

    [Export] public float ScaleDown = 0.5f;

    [Export] public float ScaleUp = 2f;

    public float Time = 0.5f;

    public Sprite Sprite { get; set; }

    public Tween Tween { get; set; }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public void GrowShrink(float scaleUp, float scaleDown, float time)
    {
        Tween.InterpolateProperty(Sprite, Property, new Vector2(scaleUp, scaleUp), new Vector2(scaleDown, scaleDown),
            time);
        Tween.Start();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (HasNode("Sprite")) Sprite = GetNode<Sprite>("Sprite");
        if (HasNode("Tween"))
        {
            Tween = GetNode<Tween>("Tween");
            Tween.Connect("tween_completed", this, nameof(OnTweenCompleted));
            GrowShrink(ScaleDown, ScaleUp, Time);
        }
    }

    public void OnTweenCompleted(Object obj, NodePath key)
    {
        if (Direction == "down")
        {
            GrowShrink(ScaleDown, ScaleUp, Time);
            Direction = "up";
        }
        else
        {
            GrowShrink(ScaleUp, ScaleDown, Time);
            Direction = "down";
        }
    }

    public virtual void OnExaminableAreaEntered(Node body)
    {
    }

    public virtual void OnExaminableAreaExited(Node body)
    {
    }
}