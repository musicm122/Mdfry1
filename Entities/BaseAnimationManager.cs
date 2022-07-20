using System;
using Godot;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities
{

    public class BaseAnimationManager : Node
    {
        protected readonly ILogger _logger = new GDLogger(LogLevelOutput.Warning);

        public Sprite Sprite { get; set; }
        protected AnimationTree AnimationTree { get; set; }
        protected AnimationPlayer BlinkAnimationPlayer { get; set; }
        protected AnimationNodeStateMachinePlayback StateMachinePlayback { get; set; }


        public virtual void UpdateAnimationBlendPositions(Vector2 movementVector)
        {
            _logger.Debug("UpdateAnimationBlendPositions arg:" + movementVector.ToString());
            UpdateAnimationBlendPosition("Idle", movementVector);
            UpdateAnimationBlendPosition("Walk", movementVector);

        }

        protected void UpdateAnimationBlendPosition(string animationName, Vector2 movementVector)
        {
            AnimationTree.Set($"parameters/{animationName}/blend_position", movementVector);
        }

        public Vector2 GetFacingDirection()
            => (Vector2)AnimationTree.Get($"parameters/Idle/blend_position");


        protected void NavToAnimation(string animationName)
        {
            try
            {
                StateMachinePlayback.Travel(animationName);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            
        }

        public void StartBlinkAnimation()
        {
            try
            {
                BlinkAnimationPlayer.Play("Start");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            
        }

        public void StopBlinkAnimation()
        {
            try
            {
                BlinkAnimationPlayer.Play("Stop");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            
        }

        public override void _Ready()
        {
            try
            {
                AnimationTree = GetNode<AnimationTree>("AnimationTree");
                AnimationTree.Active = true;
                StateMachinePlayback = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");
                BlinkAnimationPlayer = GetNode<AnimationPlayer>("./BlinkAnimationPlayer");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            
        }
    }
}