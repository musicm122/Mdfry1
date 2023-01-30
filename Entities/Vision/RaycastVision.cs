using System;
using Godot;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities.Vision;

public class RaycastVision : Mdfry1.Logic.Sight.RaycastVision
{
}

// public class RaycastVision : RayCast2D, IDebuggable<Node2D>, IVision
// {
//     private bool canSeeTarget;
//     protected ILogger _logger { get; set; } = new GDLogger(LogLevelOutput.Warning);
//     [Export] public Vector2 StartDirection { get; set; } = Vector2.Up;
//
//     [Export] public float ConeAngle { get; set; } = Mathf.Deg2Rad(30);
//
//     [Export] public float AngleBetweenRays { get; set; } = Mathf.Deg2Rad(5);
//
//     [Export] public float MaxViewDistance { get; set; } = 100;
//
//     [Export] public bool IsDebugging { get; set; }
//
//     public bool IsDebugPrintEnabled()
//     {
//         return IsDebugging;
//     }
//
//     public bool CanSeeTarget()
//     {
//         return canSeeTarget;
//     }
//
//     public Node2D OldTarget { get; set; }
//     public Node2D NewTarget { get; set; }
//     public Action<Node2D> OnTargetSeen { get; set; }
//     public Action<Node2D> OnTargetOutOfSight { get; set; }
//
//     public bool CanCheckFrame(int interval = 2)
//     {
//         return new Random().Next() % interval == 0;
//     }
//
//     public void UpdateFacingDirection(Vector2 newVelocity)
//     {
//         Rotation = Position.AngleToPoint(newVelocity.Normalized());
//     }
//
//     public void LookAtPoint(Vector2 point)
//     {
//         LookAt(point);
//     }
//
//     public override void _Ready()
//     {
//         GenerateRaycasts();
//     }
//
//     private void GenerateRaycasts()
//     {
//         var rayCount = ConeAngle / AngleBetweenRays;
//         for (var i = 0; i < rayCount; i++)
//         {
//             var ray = new RayCast2D();
//             var angle = AngleBetweenRays * (i - rayCount / 2f);
//             ray.CastTo = StartDirection.Rotated(angle) * MaxViewDistance;
//             AddChild(ray);
//             ray.Enabled = true;
//         }
//     }
//
//     private Node2D GetPlayerCollider()
//     {
//         var rays = this.GetChildrenOfType<RayCast2D>();
//         for (var i = 0; i < rays.Count; i++)
//             if (IsPlayer(rays[i]))
//                 return (Node2D)rays[i].GetCollider();
//
//         return null;
//     }
//
//     private void FlushVisionCheck()
//     {
//         if (!CanCheckFrame()) return;
//
//         // if new target is not null then target is seen
//         NewTarget = GetPlayerCollider();
//         if (NewTarget != null)
//         {
//             _logger.Debug("Player found");
//             canSeeTarget = true;
//             OnTargetSeen?.Invoke(NewTarget);
//         }
//         // if old target is not null and new target is null then target is out of sight
//         else if (OldTarget != null)
//         {
//             _logger.Debug("Player out of sight");
//             OnTargetOutOfSight?.Invoke(OldTarget);
//             canSeeTarget = false;
//         }
//
//         // set old target to new target and new target to null
//         OldTarget = NewTarget;
//         NewTarget = null;
//     }
//
//     public override void _PhysicsProcess(float delta)
//     {
//         FlushVisionCheck();
//     }
//
//     private static bool IsPlayer(RayCast2D ray)
//     {
//         return ray.IsColliding() && ((Node2D)ray.GetCollider()).Name.ToLower().Contains("player");
//     }
// }