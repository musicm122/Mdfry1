using Mdfry1.Scripts.Patterns.StateMachine;

namespace Mdfry1.Entities.PlayerState
{
    public abstract class BasePlayerState : State
    {
        public virtual void HandleInput(PlayerV2 playerV2){}
    }

    public class IdlePlayerState : BasePlayerState
    {
        
    }
}