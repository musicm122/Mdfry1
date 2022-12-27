using System;
using Godot;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Components;

public class Health : Node, IDebuggable<Node>
{
    [Signal]
    public delegate void HealthChanged(int val);

    [Signal]
    public delegate void MaxHealthChanged(int val);

    [Signal]
    public delegate void NoHealth();

    private int currentHealth = 1;

    private int maxHealth = 1;
    public Action EmptyHealthBarCallback { get; set; }
    public Action<int> HealthChangedCallback { get; set; }
    public Action<int> MaxHealthChangedCallback { get; set; }

    [Export]
    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth);
            MaxHealthChangedCallback?.Invoke(maxHealth);
            EmitSignal(nameof(MaxHealthChanged), maxHealth);
        }
    }

    [Export]
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            HealthChangedCallback?.Invoke(currentHealth);
            EmitSignal(nameof(HealthChanged), currentHealth);
            if (currentHealth <= 0)
            {
                EmptyHealthBarCallback?.Invoke();
                EmitSignal(nameof(NoHealth));
            }
        }
    }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}