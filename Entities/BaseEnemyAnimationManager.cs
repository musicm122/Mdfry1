using System.Globalization;
using Godot;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities
{

    public class BaseEnemyAnimationManager : BaseAnimationManager
    {
        public override void UpdateAnimationBlendPositions(Vector2 movementVector)
        {
            var xVal = Mathf.Round(movementVector.Normalized().x);
            UpdateAnimationBlendPosition("Run", xVal);
            UpdateAnimationBlendPosition("Walk", xVal);
            UpdateAnimationBlendPosition("Idle", xVal);
            UpdateAnimationBlendPosition("Attack", xVal);
            UpdateAnimationBlendPosition("TakeDamage", xVal);
            UpdateAnimationBlendPosition("Death", xVal);
        }

        public virtual void PlayAttackAnimation()
        {
            NavToAnimation("Attack");
        }

        public virtual void PlayDeathAnimation()
        {
            NavToAnimation("Death");
        }

        public virtual void PlayRunAnimation()
        {
            NavToAnimation("Run");
        }

        public virtual void PlayWalkAnimation()
        {
            NavToAnimation("Walk");
        }

       
        public virtual void PlayIdleAnimation()
        {
            NavToAnimation("Idle");
        }

        public virtual void PlayTakeDamageAnimation()
        {
            NavToAnimation("TakeDamage");
        }

        public override void _Ready()
        {
            base._Ready();
            Sprite = GetNode<Sprite>("../Sprite");
        }
    }
}