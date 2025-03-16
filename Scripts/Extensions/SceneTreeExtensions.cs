using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Common;
using Godot.Collections;
using Mdfry1.Entities;
using Mdfry1.Logic.Constants;
using Mdfry1.Scenes.Level;
using Mdfry1.Scripts.Constants;
using Mdfry1.Scripts.Item;

namespace Mdfry1.Scripts.Extensions;

public static class SceneTreeExtensions
{
    public static void AlertAllEnemies(this SceneTree tree)
    {
        tree.CallGroup(Groups.AllEnemies, "Alert");
    }

    public static List<EnemyV4> GetEnemies(this SceneTree tree)
    {
        return tree.GetNodesInGroup(Groups.AllEnemies).ToCSharpList<EnemyV4>();
    }
    
    public static void EnableSpawners(this SceneTree tree)
    {
        tree.CallGroup(Groups.Spawner, "EnableSpawning");
    }

    public static void DisableSpawners(this SceneTree tree)
    {
        tree.CallGroup(Groups.Spawner, "DisableSpawning");
    }

    public static void AddItem(this SceneTree tree, string name, int amt = 1)
    {
        var playerTuple = tree.GetPlayerNode();
        if (!playerTuple.Item1)
        {
            GD.PushWarning("Player node not found");
            return;
        }

        playerTuple.Item2.AddItem(name, amt);
    }

    public static void RemoveItem(this SceneTree tree, string name, int amt = 1)
    {
        tree.CallGroup(Groups.Player, "RemoveItem", name, amt);
        var playerTuple = tree.GetPlayerNode();
        if (!playerTuple.Item1)
        {
            GD.PushWarning("Player node not found");
            return;
        }

        playerTuple.Item2.RemoveItems(name, amt);
    }

    public static void AddMission(this SceneTree tree, string title)
    {
        tree.CallGroup(Groups.Player, "AddMission", title);
    }

    public static List<Examinable> GetExaminableCollection(this SceneTree tree)
    {
        return tree.GetNodesByType<Examinable>();
    }

    public static List<LockedDoor> GetLockedDoorCollection(this SceneTree tree)
    {
        return tree.GetNodesByType<LockedDoor>();
    }

    private static List<T> GetNodesByType<T>(this SceneTree tree)
    {
        var retval = new List<T>();
        var array = tree.CurrentScene.GetChildren();
        for (var index = 0; index < array.Count; index++)
        {
            var child = array[index];
            if (child is T t)
                retval.Add(t);
        }

        return retval;
    }

    private static bool HasPlayerNode(this SceneTree tree)
    {
        return tree.CurrentScene.FindNode("Player") != null;
    }

    public static (bool, PlayerV2) GetPlayerNode(this SceneTree tree)
    {
        return tree.HasPlayerNode() ? 
            (true, tree.CurrentScene.FindNode("Player") as PlayerV2) : (false, null);
    }

    public static List<EnemyV4> GetEnemyNodes(this SceneTree tree)
    {
        return tree.GetEnemies();
    }

    public static int GetEnemyCount(this SceneTree tree)
    {
        return tree.GetEnemyNodes().Count;
    }
    
    public static List<LampLight> GetLampLightNodes(this SceneTree tree)
    {
        return tree.Root.GetChildrenOfType<LampLight>();
    }


    public static (bool, List<Navigation2D>) GetNavigation2dNodes(this SceneTree tree)
    {
        var navNodes = tree.GetNodesByType<Navigation2D>();
        return navNodes.Count > 0 ? (true, navNodes) : (false, default(List<Navigation2D>));
    }

    public static Vector2 GetPlayerGlobalPosition(this SceneTree tree)
    {
        var player = tree.GetPlayerNode();
        return player.Item2.GlobalPosition;
    }

    public static (bool, DayNightCycle) GetDayNightCycle(this SceneTree tree)
    {
        var retval = tree.CurrentScene.FindNode("DayNightCycle");
        return retval != null ? (true, retval as DayNightCycle) : (false, null);
    }

    public static int GetDayCount(this SceneTree tree)
    {
        var (hasNode, node) = tree.GetDayNightCycle();
        return hasNode ? node.GetDay() : 0;
    }
}