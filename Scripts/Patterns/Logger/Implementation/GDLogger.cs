using System;
using Godot;

namespace Mdfry1.Scripts.Patterns.Logger.Implementation;

public class GDLogger : ILogger
{
    public GDLogger()
    {
        Level = LogLevelOutput.Warning;
    }

    public GDLogger(LogLevelOutput level)
    {
        Level = level;
    }

    [Export] public LogLevelOutput Level { get; set; }

    public void Debug(params string[] messages)
    {
        if (Level <= LogLevelOutput.Debug)
            foreach (var message in messages)
                GD.Print("Debug:", message);
    }

    public void Error(params string[] messages)
    {
        if (Level <= LogLevelOutput.Error)
            foreach (var message in messages)
                GD.PrintErr("Error:", message);
    }

    public void Error(Exception ex)
    {
        if (Level <= LogLevelOutput.Error) GD.PrintErr("Error:", ex.Message, ex.StackTrace, ex.Source);
    }

    public void Error(Exception ex, params string[] messages)
    {
        if (Level <= LogLevelOutput.Error)
            foreach (var message in messages)
                GD.PrintErr("Error:", message);
    }

    public void Info(params string[] messages)
    {
        if (Level <= LogLevelOutput.Info)
            foreach (var message in messages)
                GD.Print("Info:", message);
    }

    public void Warning(params string[] messages)
    {
        if (Level <= LogLevelOutput.Warning)
            foreach (var message in messages)
                GD.Print("Warning:", message);
    }
}