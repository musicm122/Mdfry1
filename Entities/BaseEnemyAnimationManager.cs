using Godot;

namespace Mdfry1.Entities
{

    public class BaseEnemyAnimationManager : BaseAnimationManager
    {


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