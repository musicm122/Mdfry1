using Godot;
using Mdfry1.Entities.Behaviors;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Entities.Components;
using Mdfry1.Entities.EnemyState;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Entities.Vision;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities
{
    public class EnemyV4 : EnemyMovableBehavior, IEnemy
    {
        [Export] public NodePath PatrolPath { get; set; }

        [Export] public EnemyBehaviorStates DefaultState { get; set; } = EnemyBehaviorStates.Wander;

        public EnemyDataStore EnemyDataStore { get; set; }

        public IDamagableBehavior Damagable { get; private set; }

        public Node2D ObstacleAvoidance { get; set; }

        public Label Cooldown { get; set; }

        private Label StateLabel { get; set; }

        private readonly StateMachine _stateMachine = new();

        public void Init()
        {
            _stateMachine.AddState(new IdleEnemyState(this));
            _stateMachine.AddState(new PatrolEnemyState(this));
            _stateMachine.AddState(new ChaseEnemyState(this));
            _stateMachine.AddState(new WanderState(this));

            if (!EnemyDataStore.LineOfSight)
            {
                _stateMachine.TransitionTo(EnemyBehaviorStates.Patrol);
            }

            if (EnemyDataStore.CurrentCoolDownCounter <= 0f)
            {
                _stateMachine.TransitionTo(DefaultState);
            }

            _stateMachine.TransitionTo(DefaultState);
        }

        public override void _Ready()
        {
            StateLabel = GetNode<Label>("StateLabel");
            EnemyDataStore = GetNode<EnemyDataStore>("Status");
            EnemyDataStore.VisionManager = GetNode<RaycastVision>("Vision");
            OnMove += OnMoveHandler;

            ObstacleAvoidance = GetNode<Node2D>("ObstacleAvoidance");

            if (EnemyDataStore.VisionManager != null)
            {
                EnemyDataStore.VisionManager.OnTargetSeen += OnTargetDetection;
                EnemyDataStore.VisionManager.OnTargetOutOfSight += OnTargetLost;
            }

            EnemyDataStore.Cooldown = GetNode<Label>("Cooldown");
            Cooldown = GetNode<Label>("Cooldown");
            EnemyDataStore.DebugLabel = this.GetNode<Label>("DebugLabel");
            Damagable = GetNode<DamagableBehavior>("Behaviors/Damagable");

            if (this.PatrolPath != null)
            {
                EnemyDataStore.Init(this.PatrolPath);
            }
            else
            {
                EnemyDataStore.DebugLabel.Text = "this.PatrolPath is null";
            }

            Damagable.Init(EnemyDataStore);
            Init();
        }

        private void OnMoveHandler(Vector2 arg1, float arg2)
        {
            EnemyDataStore.VisionManager.UpdateFacingDirection(arg1);
        }

        private void OnTargetLost(Node2D target)
        {
            EnemyDataStore.CurrentCoolDownCounter = EnemyDataStore.MaxCoolDownTime;
        }

        private void OnTargetDetection(Node2D target)
        {
            EnemyDataStore.CurrentCoolDownCounter = EnemyDataStore.MaxCoolDownTime;
            this._stateMachine.TransitionTo(EnemyBehaviorStates.ChasePlayer);
        }

        public override void _PhysicsProcess(float delta)
        {
            _stateMachine.Update(delta);
            this.StateLabel.Text = _stateMachine.CurrentState.ToString();
            if (EnemyDataStore.CurrentCoolDownCounter > 0)
            {
                EnemyDataStore.CurrentCoolDownCounter -= delta;
            }
            else
            {
                // We have lost our target and need to return to our default state. This could be more robust in the future. 
                this._stateMachine.TransitionTo(DefaultState);
            }
        }

        public void Alert()
        {
            EnemyDataStore.CurrentCoolDownCounter = EnemyDataStore.MaxCoolDownTime;
            _stateMachine.TransitionTo(EnemyBehaviorStates.ChasePlayer);
        }
    }
}