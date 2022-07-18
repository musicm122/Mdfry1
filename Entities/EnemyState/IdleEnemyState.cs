using Godot;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState
{
    public class IdleEnemyState : State
    {
        private EnemyV4 Enemy { get; set; }

        public IdleEnemyState(EnemyV4 enemy)
        {
            this.Name = EnemyBehaviorStates.Idle.GetDescription();
            Enemy = enemy;
            this.OnEnter += () => this.Logger.Debug("IdleEnemyState OnEnter called");
            this.OnExit += () => this.Logger.Debug("IdleEnemyState Exit called");
            this.OnFrame += (delta) => Enemy.Velocity = Vector2.Zero;
        }
    }
}