using System.Collections.Generic;
using System.Globalization;
using Godot;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState
{

    public class ChaseEnemyStateV2 : State
    {
        private float Threshold { get; set; } = 5f;
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
            if (Paths!=null && Paths.Count > 0)
            {
                MoveToTarget(delta);
            }
            else
            {
                  Paths = GetTargetPath(PlayerRef.GlobalPosition);
            }
        }

        private void MoveToTarget(float delta)
        {
            this.SetDebugLabel();
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
            
            
            if (IsTargetVisible(PlayerRef.GlobalPosition))
            {
                this.Enemy.EnemyDataStore.ResetCooldown();
            }
            
            if (IsInAttackRange(PlayerRef.GlobalPosition))
            {
                Enemy.AnimationManager.PlayAttackAnimation();
                this.Enemy.EnemyDataStore.ResetCooldown();
            }
        }

        private bool IsTargetVisible(Vector2 targetPosition) =>
            this.Enemy.EnemyDataStore.VisionManager.CanSeeTarget();

        private bool IsInAttackRange(Vector2 targetPosition)
        {
            var distance = targetPosition.DistanceTo(Enemy.GlobalPosition);
            var isInRange = distance < Enemy.EnemyDataStore.AttackRange;
            return isInRange;
        }

        private Stack<Vector2> GetTargetPath(Vector2 targetPosition) =>
            new(this.Nav.GetSimplePath(Enemy.GlobalPosition, targetPosition, true));

        void SetDebugLabel()
        {
            if (Enemy.IsDebugging)
            {
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
    }
}