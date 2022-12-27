using System;
using Mdfry1.Entities;

namespace Mdfry1.Scripts.Mission;

[Serializable]
public class MissionElement
{
    public Func<PlayerDataStore, bool> EvaluateCompletionState;

    public MissionElement()
    {
    }

    public MissionElement(string title, string details, bool isComplete = false)
    {
        Title = title;
        Details = details;
        IsComplete = isComplete;
    }

    public MissionElement(string title, string details, Func<PlayerDataStore, bool> evalCondition,
        bool isComplete = false)
    {
        Title = title;
        Details = details;
        IsComplete = isComplete;
        EvaluateCompletionState = evalCondition;
    }

    public bool IsComplete { get; set; }

    public string Title { get; set; }
    public string Details { get; set; }

    public override string ToString()
    {
        return $@"Mission:
        Title: {Title}
        Details: {Details}
        IsComplete: {IsComplete}";
    }
}