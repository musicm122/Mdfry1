using System;
using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.GDUtils;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Behaviors
{
    public class ShootBehavior : Node2D, IDebuggable<Node>, IShootableBehavior
    {
        public Action OnShootStart { get; set; }
        public Action OnShootComplete { get; set; }
        public Action OnNoAmmo { get; set; }
        
        public Action<Node2D> OnBulletCollision { get; set; }
        
        public float CooldownAcc { get; set; }
        
        [Export]
        public float MaxCooldown { get; set; } = 2f;
        public Position2D Muzzle { get; set; }
        public bool HasAmmo => DataStore.GetAmmoCount() > 0;

        public void Shoot()
        {
            this.OnShootStart?.Invoke();
            var instance = InstantiateBullet(DataStore.CurrentBulletPath);
            this.AddChild(instance);
            instance.SetAsToplevel(true);
            instance.GlobalPosition = this.Muzzle.GlobalPosition;
            instance.Velocity = this.DataStore.GetFacingDirection.Invoke();
            this.OnShootComplete?.Invoke();
            CooldownAcc = MaxCooldown;
        }

        public PlayerDataStore DataStore { get; set; }


        public void Init(PlayerDataStore dataStore)
        {
            this.DataStore = dataStore;
        }

        public override void _Ready()
        {
            base._Ready();
            Muzzle = GetNode<Position2D>("./Muzzle");
        }

        public Bullet InstantiateBullet(string bulletPath)
        {
            var bulletType = GD.Load<PackedScene>(bulletPath);
            var rawInstance = bulletType.Instance();
            var instance = (Bullet)rawInstance; 
            instance.OnCollision += (node) =>
            {
                if (!node.Name.ToLower().Contains("enemy")) return;
                this.OnBulletCollision?.Invoke(node);
            };
            return instance;
        }

        public bool CanShoot() => CooldownAcc <= 0;

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (!CanShoot() || !InputUtils.IsShooting())
            {
                CooldownAcc -= delta;
            }
            else
            {
                if (HasAmmo)
                {
                    Shoot();
                }
                else
                {
                    this.OnNoAmmo?.Invoke();
                }
            }
        }

        public bool IsDebugging { get; set; }
    }
}