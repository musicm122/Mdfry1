﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Mdfry1.Entities;
using Object = Godot.Object;

namespace Mdfry1.Scripts.Extensions;

public static class NodeExtensions
{
    public static Global GetGlobal(this Node node)
    {
        var temp = node.GetNode("/root/Global");
        return (Global)temp;
    }

    public static void DrawCircleArc(this Node2D node, Vector2 center, float radius, float angleFrom, float angleTo,
        Color color)
    {
        var nbPoints = 32;
        var pointsArc = new Vector2[nbPoints + 1];

        for (var i = 0; i <= nbPoints; i++)
        {
            var anglePoint = Mathf.Deg2Rad(angleFrom + i * (angleTo - angleFrom) / nbPoints - 90f);
            pointsArc[i] = center + new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
        }

        for (var i = 0; i < nbPoints - 1; i++) node.DrawLine(pointsArc[i], pointsArc[i + 1], color);
    }

    public static bool TryConnectSignal(this Node node, string signal, Object target, string methodName)
    {
        Error result;
        try
        {
            result = node.Connect(signal, target, methodName);
            if (result != Error.Ok)
            {
                var message =
                    $@"-------------------------------------
                    ConnectBodyEntered with args failed with {result.ToString()}
                    TryConnectSignal args
                    node:{node.Name ?? "null"}
                    signal:{signal ?? "null"}
                    target:{target.ToString() ?? "null"}
                    methodName :{methodName ?? "null"}
                    -------------------------------------";

                GD.PrintErr(MethodBase.GetCurrentMethod()?.Name, new ApplicationException(result.ToString()),
                    message);
                GD.PrintStack();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr(ex);
            GD.Print($@"TryConnectSignal args
                    node:{node?.Name ?? "null"}
                    signal:{signal ?? "null"}
                    target:{target.ToString() ?? "null"}
                    methodName :{methodName ?? "null"}");
            throw;
        }

        return result == Error.Ok;
    }

    public static bool TryDisconnectSignal(this Node node, string signal, Object target, string methodName)
    {
        try
        {
            if (node.HasSignal(signal))
            {
                node.Disconnect(signal, target, methodName);
                return true;
            }

            GD.Print($@"TryDisconnectSignal failed args
                node:{node.Name ?? "null"}
                signal:{signal ?? "null"}
                target:{target.ToString() ?? "null"}
                methodName :{methodName ?? "null"}");
            return false;
        }
        catch (Exception ex)
        {
            GD.Print(ex);
            GD.Print($@"TryDisconnectSignal args
                    node:{node?.Name ?? "null"}
                    signal:{signal ?? "null"}
                    target:{target.ToString() ?? "null"}
                    methodName :{methodName ?? "null"}");
            return false;
        }
    }

    public static bool IsPlayer(this Node node)
    {
        return node.Name.Contains("player", StringComparison.OrdinalIgnoreCase);
    }

    public static void Pause(this Node node)
    {
        node.GetTree().Paused = true;
    }

    public static bool IsPaused(this Node node)
    {
        return node.GetTree().Paused;
    }

    public static void Unpause(this Node node)
    {
        node.GetTree().Paused = false;
    }

    public static void TogglePause(this Node node)
    {
        node.GetTree().Paused = !node.GetTree().Paused;
    }

    public static List<T> GetChildrenOfType<T>(this Node node)
    {
        var retval = new List<T>();
        var children = node.GetChildren();
        var childCount = children.Count;
        for (var i = 0; i < childCount; i++)
            if (children[i] is T)
                retval.Add((T)children[i]);

        return retval;
    }

    public static async Task WaitForSeconds(this Node node, float seconds)
    {
        try
        {
            await node.ToSignal(node.GetTree().CreateTimer(seconds), "timeout");
        }
        catch (Exception ex)
        {
            GD.PrintErr("WaitForSeconds threw", ex);
            throw;
        }
    }

    public static string DisplayPositionData(this Node2D node, string title)
    {
        return @$"
        |-----------------------------------------------------------
        | {title} Global Position: {node.GlobalPosition.ToString()}
        | {title} Local Position: {node.Position.ToString()}
        |-----------------------------------------------------------";
    }
}