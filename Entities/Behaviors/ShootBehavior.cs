using System;
using Core.Input;
using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.GDUtils;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Behaviors;

public class ShootBehavior : Node2D, IDebuggable<Node>, IShootableBehavior
{
    public bool IsDebugging { get; set; }
    public Action OnShootStart { get; set; }
    public Action OnShootComplete { get; set; }
    public Action OnNoAmmo { get; set; }

    public Action<Node2D> OnBulletCollision { get; set; }

    public float CooldownAcc { get; set; }

    [Export] public float MaxCooldown { get; set; } = 2f;

    public Position2D Muzzle { get; set; }
    public bool HasAmmo => DataStore.GetAmmoCount() > 0;

    public void Shoot()
    {
        OnShootStart?.Invoke();
        var instance = InstantiateBullet(DataStore.CurrentBulletPath);
        AddChild(instance);
        instance.SetAsToplevel(true);
        instance.GlobalPosition = Muzzle.GlobalPosition;
        instance.Velocity = DataStore.GetFacingDirection.Invoke();
        OnShootComplete?.Invoke();
        CooldownAcc = MaxCooldown;
    }

    public PlayerDataStore DataStore { get; set; }


    public void Init(PlayerDataStore dataStore)
    {
        DataStore = dataStore;
    }

    public Bullet InstantiateBullet(string bulletPath)
    {
        var bulletType = GD.Load<PackedScene>(bulletPath);
        var rawInstance = bulletType.Instance();
        var instance = (Bullet)rawInstance;
        instance.OnCollision += node =>
        {
            if (!node.Name.ToLower().Contains("enemy")) return;
            OnBulletCollision?.Invoke(node);
        };
        return instance;
    }

    public bool CanShoot()
    {
        return CooldownAcc <= 0;
    }

    public override void _Ready()
    {
        base._Ready();
        Muzzle = GetNode<Position2D>("./Muzzle");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if (!CanShoot() || !PlayerActions.isShooting())
        {
            CooldownAcc -= delta;
        }
        else
        {
            if (HasAmmo)
                Shoot();
            else
                OnNoAmmo?.Invoke();
        }
    }
}