using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Mdfry1.Entities;

namespace Mdfry1.Scripts.Mission;

public class MissionManager : IMissionManager
{
    public delegate void AddingMissionHandler(object sender, MissionManagerEventArgs args);

    [Signal]
    public delegate void RemovingMission(MissionElement mission);

    public delegate void RemovingMissionHandler(object sender, MissionManagerEventArgs args);

    private List<MissionElement> Missions { get; } = new();

    public event EventHandler<MissionManagerEventArgs> AddMissionEvent;

    public event EventHandler<MissionManagerEventArgs> RemoveMissionEvent;

    public bool HasMission(string name)
    {
        return Missions.Any(item => item.Title == name);
    }

    public void AddIfDNE(MissionElement mission)
    {
        if (!Missions.Contains(mission))
        {
            RaiseAddingMission(mission);
            Missions.Add(mission);
        }
    }

    public void Remove(MissionElement mission)
    {
        if (Missions.Contains(mission))
        {
            RaiseRemovingMission(mission);
            Missions.Remove(mission);
        }
    }

    public void RemoveByTitle(string missionTitle)
    {
        var missionToRemove = Missions.Find(m => m.Title == missionTitle);
        RaiseRemovingMission(missionToRemove);
        Missions.Remove(missionToRemove);
    }

    public string Display()
    {
        var retval = "Points of Interest:\r\n================\r\n";
        if (Missions.Count > 0)
            for (var i = 0; i < Missions.Count; i++)
            {
                if (Missions[i].IsComplete)
                    retval += $"{Missions[i].Title} : (Complete)\r\n";
                else
                    retval += $"{Missions[i].Title} \r\n";
                retval += Missions[i].Details + "\r\n";
                retval += Missions[i].IsComplete;
            }
        else
            retval += "Empty";

        return retval;
    }

    public IEnumerable<string> Titles()
    {
        return Missions.Select(m => m.Title);
    }

    public IEnumerable<string> Details()
    {
        return Missions.Select(m => m.Details);
    }

    public IEnumerable<MissionElement> Completed()
    {
        return Missions.Where(m => m.IsComplete);
    }

    public IEnumerable<MissionElement> Uncompleted()
    {
        return Missions.Where(m => !m.IsComplete);
    }

    public int Count()
    {
        return Missions.Count;
    }

    public void EvaluateMissionsState(PlayerDataStore playerDataStore)
    {
        for (var i = 0; i < Missions.Count; i++)
            if (Missions[i].EvaluateCompletionState(playerDataStore))
                Missions[i].IsComplete = true;
    }

    protected virtual void RaiseAddingMission(MissionElement mission)
    {
        AddMissionEvent?.Invoke(this, new MissionManagerEventArgs(mission));
    }

    protected virtual void RaiseRemovingMission(MissionElement mission)
    {
        RemoveMissionEvent?.Invoke(this, new MissionManagerEventArgs(mission));
    }
}