using System.ComponentModel;

namespace Mdfry1.Scripts.Enum
{
    public enum EnemyBehaviorStates
    {
        [Description("Idle")] Idle = 0,

        [Description("Wander")] Wander,
        [Description("Patrol")] Patrol,
        [Description("Caution")] Caution,
        [Description("ChasePlayer")] ChasePlayer,
        [Description("MeleeAttackBehavior")] MeleeAttackBehavior,
        [Description("RangeAttackBehavior")] RangeAttackBehavior,
        [Description("Stun")] Stun,
        [Description("Dead")] Dead
    }
}