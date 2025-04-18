using Godot;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.EnemyState;

public class PatrolEnemyState : State
{
    public PatrolEnemyState(EnemyV4 enemy)
    {
        Name = EnemyBehaviorStates.Patrol.GetDescription();
        Enemy = enemy;

        OnEnter += () => Logger.Debug("PatrolEnemyState OnEnter called");
        OnExit += () => Logger.Debug("PatrolEnemyState Exit called");
        OnFrame += Patrol;

        if (DataStore.PatrolPath != null)
        {
            DataStore.Path = Enemy.GetNode<Path2D>(DataStore.PatrolPath);
            DataStore.PatrolPoints = DataStore.Path.Curve.GetBakedPoints();
        }
    }

    private EnemyV4 Enemy { get; }

    private EnemyDataStore DataStore => Enemy.EnemyDataStore;

    private void Patrol(float delta)
    {
        if (DataStore.PatrolPath == null || !Enemy.CanMove) return;

        var target = DataStore.PatrolPoints[DataStore.PatrolIndex];

        if (Enemy.Position.DistanceTo(target) <= 1)
        {
            DataStore.PatrolIndex = Mathf.Wrap(DataStore.PatrolIndex + 1, 0, DataStore.PatrolPoints.Length);
            target = DataStore.PatrolPoints[DataStore.PatrolIndex];
        }

        Enemy.Velocity = (target - Enemy.Position).Normalized() * Enemy.MaxSpeed;

        if (Enemy.GetSlideCount() > 0) Enemy.HandleMovableObstacleCollision(Enemy.Velocity);

        Enemy.Velocity = Enemy.MoveAndSlide(Enemy.Velocity);
    }
}