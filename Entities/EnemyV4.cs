using Common;
using Godot;
using Mdfry1.CustomResources;
using Mdfry1.Entities.Behaviors;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Entities.Components;
using Mdfry1.Entities.EnemyState;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Entities.Vision;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities
{
    public class EnemyV4 : EnemyMovableBehavior, IEnemy
    {
        
        [Export(PropertyHint.ResourceType ,"Enemy1AudioResource")]
        public EntityAudioResource AudioResource { get; set; }
        
        public IPlayAudio SoundPlayer { get; set; }
        
        public BloodSpatter BloodSpatter { get; set; }
        
        
        [Export] 
        public NodePath PatrolPath { get; set; }
        public Position2D HitboxPivot { get; set; }

        [Export] 
        public EnemyBehaviorStates DefaultState { get; set; } = EnemyBehaviorStates.Wander;
        
        public EnemyDataStore EnemyDataStore { get; set; }

        public IDamagableBehavior Damagable { get; private set; }

        public Node2D ObstacleAvoidance { get; set; }

        public Label Cooldown { get; set; }

        private Label StateLabel { get; set; }

        private readonly StateMachine _stateMachine = new();

        public EnemyAnimationManager AnimationManager { get; set; }

        void InitAudioStreams()
        {
            // TakeDamageClipPlayer = GetNode<AudioStreamPlayer>("TakeDamageClipPlayer");
            // DeathClipPlayer= GetNode<AudioStreamPlayer>("DeathClipPlayer");
            // AttackClipPlayer = GetNode<AudioStreamPlayer>("AttackClipPlayer");
        }
        

        public void Init()
        {
            _stateMachine.AddState(new IdleEnemyState(this));
            _stateMachine.AddState(new PatrolEnemyState(this));
            _stateMachine.AddState(new ChaseEnemyStateV2(this));
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
            //InitAudioStreams();
            SoundPlayer = GetNode<SoundPlayer>("Behaviors/SoundPlayer");
            BloodSpatter = GetNode<BloodSpatter>("BloodSpatter");
            HitboxPivot = GetNode<Position2D>("HitboxPiviot");
            AnimationManager = GetNode<EnemyAnimationManager>("AnimationManager");
            AnimationManager.Sprite = GetNode<Sprite>("Sprite");
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
                _logger.Debug("this.PatrolPath is null");
            }

            Damagable.Init(EnemyDataStore);
            Damagable.OnTakeDamage += OnTakeDamage;
            Damagable.EmptyHealthBarCallback += OnEmptyHealthBar;
            Damagable.HurtboxInvincibilityStartedCallback += OnHurtboxInvincibilityStarted;
            Damagable.HurtboxInvincibilityEndedCallback += OnHurtboxInvincibilityEnded;
            Init();
        }

        private void OnEmptyHealthBar()
        {
            SoundPlayer.PlaySound(AudioResource.DeathClipPath);
            _logger.Debug(this.Name + " Died");
            AnimationManager.PlayDeathAnimation();
            this.QueueFree();
        }

        private void OnHurtboxInvincibilityEnded()
        {
            _logger.Debug(this.Name + " OnHurtboxInvincibilityEnded");
        }

        private void OnHurtboxInvincibilityStarted()
        {
            _logger.Debug(this.Name + " OnHurtboxInvincibilityStarted");
        }

        private void OnTakeDamage(Node sender, Vector2 damageForce)
        {
            _logger.Debug(this.Name + " took damage");
            
            SoundPlayer.PlaySound(AudioResource.TakeDamageClipPath);
            
            AnimationManager.PlayTakeDamageAnimation();
            var bloodSpatter = (BloodSpatter)GD.Load<PackedScene>("res://Entities/Effects/BloodSpatter.tscn").Instance();
            bloodSpatter.GlobalPosition = GlobalPosition;
            GetTree().Root.AddChild(bloodSpatter);
            
            MoveAndSlide(damageForce);
            Alert();
        }

        private void OnMoveHandler(Vector2 velocity, float delta)
        {
            AnimationManager?.UpdateAnimationBlendPositions(velocity);
            EnemyDataStore.VisionManager?.UpdateFacingDirection(velocity);

            if (_stateMachine.CurrentState.Name == "ChasePlayer")
            {
                AnimationManager?.PlayRunAnimation();
            }
            else
            {
                AnimationManager?.PlayWalkAnimation();
            }
        }

        private void OnTargetLost(Node2D target)
        {
            EnemyDataStore.CurrentCoolDownCounter = EnemyDataStore.MaxCoolDownTime;
        }

        private void OnTargetDetection(Node2D target)
        {
            Alert();
        }

        public override void _PhysicsProcess(float delta)
        {
            _stateMachine.Update(delta);
            if (IsDebugging)
            {
                this.StateLabel.Text = _stateMachine.CurrentState.ToString();
            }

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