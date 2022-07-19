using System;
using Godot;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Item.ItemTypes;
using Mdfry1.Scripts.Mission;

namespace Mdfry1.Entities
{
    public class PlayerDataStore
    {
        public Func<Vector2> GetFacingDirection { get; set; }

        public Health PlayerStatus { get; set; } = new Health();
        
        public string CurrentBulletPath { get; set; } ="res://Entities/Bullet.tscn";
        
        public MissionManager MissionManager { get; set; } = new MissionManager();

        public Inventory Inventory { get; set; } = new Inventory();

        public int GetAmmoCount() => Inventory.CountOfType("Ammo");
        
        public void DecrementAmmo() => Inventory.RemoveAmount("Ammo",1);

    }
}