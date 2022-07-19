using Godot;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerAnimationManager : BaseAnimationManager
    {
        public void PlayShootAnimation(Vector2 currVelocity)
        {
            NavToAnimation("Shoot");
        }
        
        public override void UpdateAnimationBlendPositions(Vector2 movementVector)
        {
            _logger.Debug("UpdateAnimationBlendPositions arg:" + movementVector.ToString());
            base.UpdateAnimationBlendPositions(movementVector);
            UpdateAnimationBlendPosition("Roll", movementVector);
            UpdateAnimationBlendPosition("Shoot", movementVector);
            UpdateAnimationBlendPosition("EmptyClip", movementVector);
        }
        
        public void PlayEmptyClipAnimation(Vector2 currVelocity)
        {
            NavToAnimation("EmptyClip");
        }
        
        public void PlayRollAnimation(Vector2 currVelocity)
        {
            NavToAnimation("Roll");
        }

        public void PlayIdleAnimation(Vector2 currVelocity)
        {
            NavToAnimation("Idle");
        }

        public void PlayWalkAnimation(Vector2 currVelocity)
        {
            NavToAnimation("Walk");
        }

        public override void _Ready()
        {
            AnimationTree = GetNode<AnimationTree>("AnimationTree");
            AnimationTree.Active = true;
            StateMachinePlayback = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");
            BlinkAnimationPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
        }
    }
}