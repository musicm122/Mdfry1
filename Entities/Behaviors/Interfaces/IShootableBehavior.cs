using System;
using Godot;

namespace Mdfry1.Entities.Behaviors.Interfaces;

public interface IShootableBehavior
{
    public Action OnShootStart { get; set; }
    public Action OnShootComplete { get; set; }
    public Action<Node2D> OnBulletCollision { get; set; }

    public Action OnNoAmmo { get; set; }

    public float CooldownAcc { get; set; }

    public float MaxCooldown { get; set; }

    public Position2D Muzzle { get; set; }
    public bool HasAmmo => DataStore.GetAmmoCount() > 0;

    public PlayerDataStore DataStore { get; set; }

    public bool CanShoot()
    {
        return CooldownAcc <= 0f && HasAmmo;
    }

    public void Shoot();
    public void Init(PlayerDataStore dataStore);
    public Bullet InstantiateBullet(string bulletPath);
}