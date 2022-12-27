using System;
using Godot;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Mission;

namespace Mdfry1.Entities;

public class PlayerDataStore
{
    public Func<Vector2> GetFacingDirection { get; set; }

    public Health PlayerStatus { get; set; } = new();

    public string CurrentBulletPath { get; set; } = "res://Entities/Bullet.tscn";

    public MissionManager MissionManager { get; set; } = new();

    public Inventory Inventory { get; set; } = new();

    public int GetAmmoCount()
    {
        return Inventory.CountOfType("Ammo");
    }

    public void DecrementAmmo()
    {
        Inventory.RemoveAmount("Ammo");
    }
}