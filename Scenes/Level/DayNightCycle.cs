using System;
using System.Globalization;
using Godot;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

// Adapted from 
//https://github.com/bitbrain/godot-tutorials/blob/main/2d-lighting/demo/DayNightCycle.gd
// and I'm too tired to clean up formatting and casing. I'm sorry.


namespace Mdfry1.Scenes.Level;

public class DayNightCycle : CanvasModulate
{
    private readonly ILogger _logger = new GDLogger
    {
        Level = LogLevelOutput.Debug
    };

    [Export] private bool IsEnemySpawningEnabled { get; set; } = true;
    
    [Export] private Color _dayColor = new("#ffffff");

    [Export] private Color _eveningColor = new("#ff3300");

    [Export] private LogLevelOutput _logLevel = LogLevelOutput.Debug;

    [Export(PropertyHint.ColorNoAlpha)] private Color _nightColor = new("#091d3a");

    [Export(PropertyHint.Range, "1,60")] public float DayInSeconds { get; set; } = 40f;

    [Export(PropertyHint.Range, "0.01,1")] public float ThresholdForSpawning { get; set; } = 0.35f;

    [Export]
    public LogLevelOutput LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            _logger.Level = _logLevel;
        }
    }

    public Action<int> DayTick { get; set; }

    public float TotalTime { get; set; }

    [Export] public float TimeScale { get; set; } = 0.1f; // 0.1 == Roughly 40 seconds in a day

    public float Time { get; set; }
    public int LastDay { get; set; }

    private Color GetSourceColor(float toColorVal)
    {
        return _nightColor.LinearInterpolate(_eveningColor, toColorVal);
    }

    private Color GetTargetColor(float toColorVal)
    {
        return _eveningColor.LinearInterpolate(_dayColor, toColorVal);
    }

    public int GetDay()
    {
        const float offset = 0.15f;
        var temp = (int)(offset + Mathf.Ceil(0.5f * Time) / Mathf.Pi);
        return temp + 1;
    }

    /// <summary>
    ///     if we are at l
    /// </summary>
    /// <returns></returns>
    private bool ToggleSpawnCheck()
    {
        if (!IsEnemySpawningEnabled) return false; 
        _logger.Debug(
            $" ({TotalTime.ToString(CultureInfo.InvariantCulture)} / {DayInSeconds.ToString(CultureInfo.InvariantCulture)}) : {TotalTime / DayInSeconds} : > {ThresholdForSpawning.ToString(CultureInfo.InvariantCulture)}");
        return TotalTime / DayInSeconds > this.ThresholdForSpawning;
    }

    public override void _Process(float delta)
    {
        if (ToggleSpawnCheck())
        {
            _logger.Debug("Enabling Spawning");
            GetTree().EnableSpawners();
        }
        else
        {
            _logger.Debug("Disabling Spawning");
            GetTree().DisableSpawners();
        }

        Time += delta * TimeScale;
        TotalTime += delta;
        var val = (Mathf.Sin(Time) + 1) / 2;
        Color = GetSourceColor(val).LinearInterpolate(GetTargetColor(val), val);
        var newDay = GetDay();
        if (newDay == LastDay) return;
        LastDay = newDay;
        DayTick?.Invoke(newDay);
    }
}