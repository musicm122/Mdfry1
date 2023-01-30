using System.Globalization;
using Godot;
using Mdfry1.Entities.Interfaces;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities.Components;

public class EnemyDataStore : Health
{
    [Export] public float AttackRange { get; set; }

    public Mdfry1.Logic.Sight.IVision VisionManager { get; set; }

    public ILogger Logger { get; set; } = new GDLogger(LogLevelOutput.Warning);
    public NodePath PatrolPath { get; private set; }

    public Label Cooldown { get; set; }

    public Label DebugLabel { get; set; }

    public Path2D Path { get; set; }

    public Vector2[] PatrolPoints { get; set; }

    public bool LineOfSight { get; set; }

    public int PatrolIndex { get; set; } = 0;

    public float CurrentCoolDownCounter { get; set; }

    [Export] public float MaxCoolDownTime { get; set; } = 10f;

    public void Init(NodePath patrolPath)
    {
        PatrolPath = patrolPath;
    }

    public void ResetCooldown()
    {
        CurrentCoolDownCounter = MaxCoolDownTime;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (CurrentCoolDownCounter > 0f)
        {
            Logger.Debug("CurrentCoolDownCounter = " + CurrentCoolDownCounter);
            CurrentCoolDownCounter -= delta;
            if (IsDebugging)
                Cooldown.Text =
                    $"Cooling Down in {CurrentCoolDownCounter.ToString(CultureInfo.InvariantCulture)} seconds";
        }
        else
        {
            Cooldown.Text = string.Empty;
        }
    }
}