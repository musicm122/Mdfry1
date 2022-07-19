using System;
using Godot;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities
{
    public class Bullet : HitBox
    {
        public Action OnCollision { get; set; }
        
        [Export]
        public int AttackPower { get; set; }
        
        [Export]
        public float Speed { get; set; }

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

        public void InitBullet(Vector2 position,Vector2 direction)
        {
            this.Position = position;
            this.Rotation = direction.Angle();
            this.Velocity = direction * Speed;
        }

        public override void _PhysicsProcess(float delta)
        {
            this.Translate(Velocity * Speed * delta);
        }
    }
}