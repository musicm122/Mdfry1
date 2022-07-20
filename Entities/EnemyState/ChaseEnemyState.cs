using System.Collections.Generic;
using Godot;
using Mdfry1.Entities.Components;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState
{
    public class ChaseEnemyState : State
    {
        private Navigation2D Nav { get; }
        private EnemyV4 Enemy { get; }
        private PlayerV2 PlayerRef { get; }
        private EnemyDataStore DataStore => Enemy.EnemyDataStore;
        private IVision VisionManager => Enemy.EnemyDataStore.VisionManager;

        public ChaseEnemyState(EnemyV4 enemy)
        {
            this.Name = EnemyBehaviorStates.ChasePlayer.GetDescription();
            Enemy = enemy;
            (var hasPlayer, PlayerRef) = Enemy.GetTree().GetPlayerNode();
            if (!hasPlayer)
            {
                Logger.Error("Player ref not found on scene tree");
            }

            var (hasNav, navNodes) = Enemy.GetTree().GetNavigation2dNodes();
            if (hasNav && navNodes != null)
            {
                Nav = navNodes[0];
            }

            this.OnEnter += OnEnterState;
            this.OnExit += OnExitState;
            this.OnFrame += ChasePlayer;
        }

        private void OnEnterState()
        {
            this.Logger.Debug("ChaseEnemyState OnEnter called");
        }

        private void OnExitState()
        {
            this.Logger.Debug("ChaseEnemyState Exit called");
        }

        private void ChasePlayer(float delta)
        {
            if (Nav != null)
            {
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
                        var globalNewPosition = Enemy.GlobalPosition + globalDisplacement;
                        var localNewPosition = Enemy.ToLocal(globalNewPosition);
                        //VisionManager.UpdateFacingDirection(localNewPosition);
                        Enemy.MoveAndSlide(globalDisplacement / delta);
                        //Enemy.Move(globalDisplacement / delta);
                    }
                    else
                    {
                        _ = enemyNavPath.Pop();
                        var globalNewPosition = nextPoint;
                        var localNewPosition = Enemy.ToLocal(globalNewPosition);
                        //VisionManager.UpdateFacingDirection(localNewPosition);
                        if (Enemy.GetSlideCount() > 0)
                        {
                            Enemy.HandleMovableObstacleCollision(globalDirection);
                        }

                        Enemy.GlobalPosition = globalNewPosition;
                    }

                    distanceToWalk -= globalDistance;
                }
            }
            else
            {
                Logger.Error("Navigation2D not found");
            }

            if (DataStore.CurrentCoolDownCounter > 0)
            {
                DataStore.CurrentCoolDownCounter -= delta;
            }

            if (IsTargetVisible(PlayerRef.GlobalPosition))
            {
                Logger.Debug("ChaseEnemyState: Player is visible ");
                this.Enemy.EnemyDataStore.ResetCooldown();
                if (IsInAttackRange(PlayerRef.GlobalPosition))
                {
                    Logger.Debug("ChaseEnemyState: Player is in attack range ");
                    Enemy.AnimationManager.PlayAttackAnimation();
                }
            }
        }

        private void UpdateFacingDir(Vector2 localNewPosition)
        {
            VisionManager.UpdateFacingDirection(Enemy.Velocity);
            FlipCheck(Enemy.Velocity);
        }
        
        void FlipCheck(Vector2 velocity) 
        {
            //assumes default flip is facing left
            Enemy.AnimationManager.Sprite.FlipH = velocity.x > 0;
            Enemy.HitboxPivot.Scale =  velocity.x > 0 ? new Vector2(1,1): new Vector2(-1,1);
        }

        private bool IsTargetVisible(Vector2 targetPosition) =>
            this.Enemy.EnemyDataStore.VisionManager.CanSeeTarget();

        private bool IsInAttackRange(Vector2 targetPosition)
        {
            var distance = targetPosition.DistanceTo(Enemy.GlobalPosition);
            Logger.Debug("ChaseEnemyState: Player distance is " + distance.ToString());
            var isInRange = distance < Enemy.EnemyDataStore.AttackRange;
            return isInRange;
        }

        private Stack<Vector2> GetTargetPath(Vector2 targetPosition) =>
            new Stack<Vector2>(this.Nav.GetSimplePath(Enemy.GlobalPosition, targetPosition, false));
    }
}