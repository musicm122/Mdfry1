using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Entities.Components;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;

namespace Mdfry1.Entities.Behaviors;

public class DamagableBehavior : Node2D, IDebuggable<Node>, IDamagableBehavior
{
    private static readonly List<string> DefaultDamagableNames = new() { "hitbox", "spikes" };
    private readonly ILogger _logger = new GDLogger(LogLevelOutput.Warning);

    [Export] public List<string> DamagableNames { get; set; } = DefaultDamagableNames;

    public bool IsDead { get; set; }

    private Health Status { get; set; }

    private Hurtbox HurtBox { get; set; }

    public Action<Node, Vector2> OnTakeDamage { get; set; }
    public Action EmptyHealthBarCallback { get; set; }
    public Action HurtboxInvincibilityStartedCallback { get; set; }
    public Action HurtboxInvincibilityEndedCallback { get; set; }
    public Action<int> HealthChangedCallback { get; set; }
    public Action<int> MaxHealthChangedCallback { get; set; }

    public void OnHurtboxAreaEntered(Node body)
    {
        try
        {
            _logger.Debug($"OnHurtboxAreaEntered({body.Name})");
            if (HurtBox.IsInvincible) return;
            if (!DamagableNames.Contains(body.Name.ToLower())) return;
            HurtBox.StartInvincibility();
            var hitBox = (HitBox)body;
            var force = (GlobalPosition - hitBox.GlobalPosition) * hitBox.EffectForce;
            OnTakeDamage?.Invoke(body, force);
            _logger.Debug($"OnHurtboxAreaEntered: hitBox.Damage={hitBox.Damage.ToString()}");
            Status.CurrentHealth -= hitBox.Damage;
            _logger.Debug(
                $"OnHurtboxAreaEntered: Current Health ={Status.CurrentHealth.ToString(CultureInfo.InvariantCulture)}");
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    public void OnEmptyHealthBar()
    {
        _logger.Debug($"{Name} OnEmptyHealthBar called");
        IsDead = true;
        EmptyHealthBarCallback?.Invoke();
    }

    public void OnHurtboxInvincibilityStarted()
    {
        _logger.Debug($"{Name} OnHurtboxInvincibilityStarted ");
        HurtboxInvincibilityStartedCallback?.Invoke();
    }

    public void OnHurtboxInvincibilityEnded()
    {
        _logger.Debug($"{Name} OnHurtboxInvincibilityEnded ");
        HurtboxInvincibilityEndedCallback?.Invoke();
    }

    public void OnHealthChanged(int health)
    {
        _logger.Debug($"{Name} OnHealthChanged {health.ToString()}");
        HealthChangedCallback?.Invoke(health);
    }

    public void OnMaxHealthChanged(int health)
    {
        _logger.Debug($"{Name} OnMaxHealthChanged {health.ToString()}");
        MaxHealthChangedCallback?.Invoke(health);
    }

    public void Init(Health status)
    {
        Status = status;
        HurtBox = GetNode<Hurtbox>("Hurtbox");
        RegisterHealthSignals();
        RegisterHurtBoxSignals();
    }

    public override void _Ready()
    {
        HurtBox = GetNode<Hurtbox>("Hurtbox");
    }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    private void RegisterHealthSignals()
    {
        _logger.Debug($"{Name} RegisterHealthSignals called");
        Status.Connect(nameof(Health.NoHealth), this, nameof(OnEmptyHealthBar));
        Status.Connect(nameof(Health.HealthChanged), this, nameof(OnHealthChanged));
        Status.Connect(nameof(Health.MaxHealthChanged), this, nameof(OnMaxHealthChanged));
    }

    private void RegisterHurtBoxSignals()
    {
        _logger.Debug($"{Name} RegisterHurtBoxSignals called");
        if (!HurtBox.TryConnectSignal("area_entered", this, nameof(OnHurtboxAreaEntered)))
        {
            var arg = $"TryConnectSignal('area_entered', {Name}, {nameof(OnHurtboxAreaEntered)})";
            _logger.Error($"Attempt to register Hurtbox's signal with args {arg} failed");
        }
        
        /*
         this warning is a f# to C# issue. 
         if I don't tell the compiler what type it is then it won't yell at me.
         https://stackoverflow.com/questions/55330154/how-to-adapt-actionstring-into-fsharpfuncstring-unit 
        */
        HurtBox.OnInvincibleStartedEventHandler.AddHandler((_, _) =>OnHurtboxInvincibilityStarted());
        HurtBox.OnInvincibleEndedEventHandler.AddHandler((_, _) => OnHurtboxInvincibilityStarted());
    }
}