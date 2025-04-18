﻿using System.Collections.Generic;
using Godot;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState;

public class ChaseEnemyStateV2 : State
{
    public ChaseEnemyStateV2(EnemyV4 enemy)
    {
        Name = EnemyBehaviorStates.ChasePlayer.GetDescription();
        Enemy = enemy;
        Nav = Enemy.GetTree().GetNavigation2dNodes().Item2[0];
        (var hasPlayer, PlayerRef) = Enemy.GetTree().GetPlayerNode();
        if (hasPlayer) Paths = GetTargetPath(PlayerRef.GlobalPosition);

        OnEnter += OnEnterState;
        OnExit += OnExitState;
        OnFrame += OnPhysicsProcess;
    }

    private float Threshold { get; } = 5f;
    private EnemyV4 Enemy { get; }
    private PlayerV2 PlayerRef { get; }
    private Navigation2D Nav { get; }

    private Stack<Vector2> Paths { get; set; } = new();

    private void OnEnterState()
    {
        Logger.Debug("ChaseEnemyState OnEnter called");
    }

    private void OnExitState()
    {
        Logger.Debug("ChaseEnemyState Exit called");
    }

    private void OnPhysicsProcess(float delta)
    {
        if (Paths != null && Paths.Count > 0)
            MoveToTarget(delta);
        else
            Paths = GetTargetPath(PlayerRef.GlobalPosition);
    }

    private void MoveToTarget(float delta)
    {
        SetDebugLabel();
        if (Enemy.GlobalPosition.DistanceTo(Paths.Peek()) < Threshold)
        {
            Paths.Pop();
        }
        else
        {
            var currentPath = Paths.Pop();
            var dir = Enemy.GlobalPosition.DirectionTo(currentPath);
            Enemy.Velocity = dir * (Enemy.MoveMultiplier * Enemy.MaxSpeed);
            Enemy.Move(delta);
        }


        if (IsTargetVisible(PlayerRef.GlobalPosition)) Enemy.EnemyDataStore.ResetCooldown();

        if (IsInAttackRange(PlayerRef.GlobalPosition))
        {
            Enemy.SoundPlayer.PlaySound(Enemy.AudioResource.DeathClipPath);
            Enemy.AnimationManager.PlayAttackAnimation();
            Enemy.EnemyDataStore.ResetCooldown();
        }
    }

    private bool IsTargetVisible(Vector2 targetPosition)
    {
        return Enemy.EnemyDataStore.VisionManager.CanSeeTarget();
    }

    private bool IsInAttackRange(Vector2 targetPosition)
    {
        var distance = targetPosition.DistanceTo(Enemy.GlobalPosition);
        var isInRange = distance < Enemy.EnemyDataStore.AttackRange;
        return isInRange;
    }

    private Stack<Vector2> GetTargetPath(Vector2 targetPosition)
    {
        return new Stack<Vector2>(Nav.GetSimplePath(Enemy.GlobalPosition, targetPosition));
    }

    private void SetDebugLabel()
    {
        if (Enemy.IsDebugging)
            Enemy.EnemyDataStore.DebugLabel.Text =
                @$"
                    |-----------------------------------------------------------
                    | Agent Position : {Enemy.Position.ToString()}
                    | Agent Global Position : {Enemy.GlobalPosition.ToString()}
                    | Velocity : {Enemy.Velocity.ToString()}
                    |-----------------------------------------------------------
                    | Paths Count: {Paths.Count.ToString()}
                    | Paths Count: {Paths.Peek().ToString()}
                    |-----------------------------------------------------------
                    | Player Position : {PlayerRef.Position.ToString()}
                    | Player Global Position : {PlayerRef.GlobalPosition.ToString()}
                    |-----------------------------------------------------------";
    }
}