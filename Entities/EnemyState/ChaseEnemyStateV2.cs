using System.Collections.Generic;
using Godot;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState;

public class ChaseEnemyStateV2 : State
{
    private float Threshold { get; set; } = 16f;
    private EnemyV4 Enemy { get; }
    private PlayerV2 PlayerRef { get; }
    private Navigation2D Nav { get; }


    private Stack<Vector2> Paths { get; set; } = new Stack<Vector2>();

    public ChaseEnemyStateV2(EnemyV4 enemy)
    {
        this.Name = EnemyBehaviorStates.ChasePlayer.GetDescription();
        Enemy = enemy;
        Nav = Enemy.GetTree().GetNavigation2dNodes().Item2[0];
        (var hasPlayer, PlayerRef) = Enemy.GetTree().GetPlayerNode();
        if (hasPlayer)
        {
            Paths = GetTargetPath(PlayerRef.GlobalPosition);
        }

        this.OnEnter += OnEnterState;
        this.OnExit += OnExitState;
        this.OnFrame += OnPhysicsProcess;
    }

    private void OnEnterState()
    {
        this.Logger.Debug("ChaseEnemyState OnEnter called");
    }

    private void OnExitState()
    {
        this.Logger.Debug("ChaseEnemyState Exit called");
    }

    void OnPhysicsProcess(float delta)
    {
        if (Paths.Count > 0)
        {
            MoveToTarget(delta);
        }
    }

    private void MoveToTarget(float delta)
    {
        if (Enemy.GlobalPosition.DistanceTo(Paths.Peek()) < Threshold)
        {
            Paths.Pop();
        }

        var dir = Enemy.GlobalPosition.DirectionTo(Paths.Peek());
        Enemy.Velocity = dir * Enemy.MaxSpeed;
        Enemy.Move(delta);
        if (IsTargetVisible(PlayerRef.GlobalPosition))
        {
            this.Enemy.EnemyDataStore.ResetCooldown();
        }
    }

    private bool IsTargetVisible(Vector2 targetPosition) =>
        this.Enemy.EnemyDataStore.VisionManager.CanSeeTarget();


    private Stack<Vector2> GetTargetPath(Vector2 targetPosition) =>
        new Stack<Vector2>(this.Nav.GetSimplePath(Enemy.GlobalPosition, targetPosition, false));
}