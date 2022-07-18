using Godot;

namespace Mdfry1.Tools
{
    [Tool]
    public class DebugNode : Node2D
    {
        [Export]
        public float Radius { get; set; } = 10f;

        [Export]
        public Color Color { get; set; } = new Color(255, 0, 0);

        public override void _Draw()
        {
            DrawCircle(new Vector2(0, 0), Radius, Color);
        }


    }
}
