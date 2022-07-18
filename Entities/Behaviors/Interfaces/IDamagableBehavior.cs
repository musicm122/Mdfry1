using System;
using Godot;
using Mdfry1.Entities.Components;

namespace Mdfry1.Entities.Behaviors.Interfaces
{
    public interface IDamagableBehavior
    {
        Action EmptyHealthBarCallback { get; set; }
        Action<int> HealthChangedCallback { get; set; }
        Action HurtboxInvincibilityEndedCallback { get; set; }
        Action HurtboxInvincibilityStartedCallback { get; set; }
        bool IsDebugging { get; set; }
        Action<int> MaxHealthChangedCallback { get; set; }
        Action<Node, Vector2> OnTakeDamage { get; set; }

        void Init(Health status);
        bool IsDebugPrintEnabled();
        void OnEmptyHealthBar();
        void OnHealthChanged(int health);
        void OnHurtboxAreaEntered(Node body);
        void OnHurtboxInvincibilityEnded();
        void OnHurtboxInvincibilityStarted();
        void OnMaxHealthChanged(int health);
        void _Ready();
    }
}