using System.Collections.Generic;
using Godot;
using Mdfry1.Entities.Components;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState;

public class ChaseEnemyState : State
{
    public ChaseEnemyState(EnemyV4 enemy)
    {
        Name = EnemyBehaviorStates.ChasePlayer.GetDescription();
        Enemy = enemy;
        (var hasPlayer, PlayerRef) = Enemy.GetTree().GetPlayerNode();
        if (!hasPlayer) Logger.Error("Player ref not found on scene tree");

        var (hasNav, navNodes) = Enemy.GetTree().GetNavigation2dNodes();
        if (hasNav && navNodes != null) Nav = navNodes[0];

        OnEnter += OnEnterState;
        OnExit += OnExitState;
        OnFrame += ChasePlayer;
    }

    private Navigation2D Nav { get; }
    private EnemyV4 Enemy { get; }
    private PlayerV2 PlayerRef { get; }
    private EnemyDataStore DataStore => Enemy.EnemyDataStore;
    private Mdfry1.Logic.Sight.IVision VisionManager => Enemy.EnemyDataStore.VisionManager;

    private void OnEnterState()
    {
        Logger.Debug("ChaseEnemyState OnEnter called");
    }

    private void OnExitState()
    {
        Logger.Debug("ChaseEnemyState Exit called");
    }

    private void ChasePlayer(float delta)
    {
        if (Nav == null)
        {
            Logger.Error("Navigation2D not found");
            return;
        }

        var from = Nav.ToLocal(Enemy.GlobalPosition);
        var to = Nav.ToLocal(PlayerRef.GlobalPosition);
        var paths = Nav.GetSimplePath(from, to);

        var enemyNavPath = new Stack<Vector2>(paths);
        var distanceToWalk = Enemy.MaxSpeed * delta;

        while (distanceToWalk > 0f && enemyNavPath.Count > 0)
        {
            var nextPoint = Nav.ToGlobal(enemyNavPath.Peek());
            var globalDirection = Enemy.GlobalPosition.DirectionTo(nextPoint);
            var globalDistance = Enemy.GlobalPosition.DistanceTo(nextPoint);
            if (distanceToWalk <= globalDistance)
            {
                var globalDisplacement = globalDirection * distanceToWalk;
                Enemy.MoveAndSlide(globalDisplacement / delta);
            }
            else
            {
                _ = enemyNavPath.Pop();
                var globalNewPosition = nextPoint;

                if (Enemy.GetSlideCount() > 0) Enemy.HandleMovableObstacleCollision(globalDirection);

                Enemy.GlobalPosition = globalNewPosition;
            }

            distanceToWalk -= globalDistance;
        }

        if (IsTargetVisible(PlayerRef.GlobalPosition))
        {
            Logger.Debug("ChaseEnemyState: Player is visible ");
            Enemy.EnemyDataStore.ResetCooldown();
            if (IsInAttackRange(PlayerRef.GlobalPosition))
            {
                Logger.Debug("ChaseEnemyState: Player is in attack range ");
                Enemy.AnimationManager.PlayAttackAnimation();
            }
        }

        if (DataStore.CurrentCoolDownCounter <= 0) return;
        DataStore.CurrentCoolDownCounter -= delta;
    }

    private bool IsTargetVisible(Vector2 targetPosition)
    {
        return VisionManager.CanSeeTarget();
    }

    private bool IsInAttackRange(Vector2 targetPosition)
    {
        var distance = targetPosition.DistanceTo(Enemy.GlobalPosition);
        Logger.Debug("ChaseEnemyState: Player distance is " + distance);
        var isInRange = distance < Enemy.EnemyDataStore.AttackRange;
        return isInRange;
    }
}