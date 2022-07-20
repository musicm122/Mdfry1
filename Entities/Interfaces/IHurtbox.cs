using Godot;

namespace Mdfry1.Entities.Interfaces
{
    public interface IHurtbox
    {
        CollisionShape2D CollisionShape { get; set; }
        bool IsInvincible { get; set; }
        bool IsDebugging { get; set; }
        Timer Timer { get; set; }

        bool IsDebugPrintEnabled();
        void OnHurtboxInvincibilityEnded();
        void OnHurtboxInvincibilityStarted();
        void OnTimerTimeout();
        
        void StartInvincibility();
        void _Ready();
    }
}