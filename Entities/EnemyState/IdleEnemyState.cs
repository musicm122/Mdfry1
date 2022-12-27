using Godot;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState;

public class IdleEnemyState : State
{
    public IdleEnemyState(EnemyV4 enemy)
    {
        Name = EnemyBehaviorStates.Idle.GetDescription();
        Enemy = enemy;
        OnEnter += () => Logger.Debug("IdleEnemyState OnEnter called");
        OnExit += () => Logger.Debug("IdleEnemyState Exit called");
        OnFrame += delta => Enemy.Velocity = Vector2.Zero;
    }

    private EnemyV4 Enemy { get; }
}