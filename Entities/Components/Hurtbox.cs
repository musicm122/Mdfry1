using Godot;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities.Components;

public class Hurtbox : Area2D, IDebuggable<Node>, IHurtbox
{
    [Signal]
    public delegate void InvincibilityEnded();

    [Signal]
    public delegate void InvincibilityStarted();

    private bool _isInvincible;

    private readonly ILogger _logger = new GDLogger(LogLevelOutput.Warning);

    [Export] public float InvincibleTime { get; set; } = 0.6f;

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public bool IsInvincible
    {
        get => _isInvincible;
        set
        {
            //todo: figure out why this is causing a stack overflow when shooting enemy
            _isInvincible = value;
            EmitSignal(value ? nameof(InvincibilityStarted) : nameof(InvincibilityEnded));
        }
    }

    public Timer Timer { get; set; }
    public CollisionShape2D CollisionShape { get; set; }

    public void StartInvincibility()
    {
        _logger.Debug($"{nameof(Hurtbox)}: Starting invincibility");
        IsInvincible = true;
        Timer.Start(InvincibleTime);
    }

    public void OnTimerTimeout()
    {
        Timer.Stop();
        _logger.Debug($"{nameof(Hurtbox)}: OnTimerTimeout");
        IsInvincible = false;
    }

    public void OnHurtboxInvincibilityStarted()
    {
        CollisionShape.SetDeferred("disabled", true);
    }

    public void OnHurtboxInvincibilityEnded()
    {
        CollisionShape.Disabled = false;
    }

    public override void _Ready()
    {
        Timer = GetNode<Timer>("Timer");
        Timer.Connect("timeout", this, nameof(OnTimerTimeout));

        CollisionShape = GetNode<CollisionShape2D>("CollisionShape");
        Connect(nameof(InvincibilityStarted), this, nameof(OnHurtboxInvincibilityStarted));
        Connect(nameof(InvincibilityEnded), this, nameof(OnHurtboxInvincibilityEnded));
    }
}