using System;
using Godot;
using Mdfry1.Entities.Components;

namespace Mdfry1.Entities
{
    public class Bullet : HitBox
    {
        public Action<Node2D> OnCollision { get; set; }

        [Export] public float Speed { get; set; }

        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public Timer LifeTime { get; set; }

        public override void _Ready()
        {
            LifeTime = GetNode<Timer>("Lifetime");
            LifeTime.Connect("timeout", this, nameof(OnTimeout));
            LifeTime.Start();
        }

        public void Clear()
        {
            QueueFree();
        }

        private void OnTimeout()
        {
            Clear();
        }

        public override void _PhysicsProcess(float delta)
        {
            Translate(Velocity * Speed * delta);
        }
    }
}