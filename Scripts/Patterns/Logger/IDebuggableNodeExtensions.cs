using System;
using System.Runtime.CompilerServices;
using Godot;

namespace Mdfry1.Scripts.Patterns.Logger;

public static class IDebuggableNodeExtensions
{
    public static void PrintError<T>(this IDebuggable<T> node, Exception ex, string message) where T : Node
    {
        GD.PrintErr(((Node)node).Name, ex, message);
        GD.PrintStack();
    }

    public static void PrintError<T>(this IDebuggable<T> node, Error err, string message) where T : Node
    {
        GD.PrintErr(((Node)node).Name, err, message);
        GD.PrintStack();
    }

    public static void Print<T>(this IDebuggable<T> node, string message) where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"${message}");
    }

    public static void Print<T>(this IDebuggable<T> node, string message, [CallerMemberName] string callerName = "")
        where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"{callerName}:${message}");
    }

    public static void Print<T>(this IDebuggable<T> node, int message, [CallerMemberName] string callerName = "")
        where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"{callerName}:${message}");
    }

    public static void Print<T>(this IDebuggable<T> node, float message, [CallerMemberName] string callerName = "")
        where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"{callerName}:${message}");
    }

    public static void Print<T>(this IDebuggable<T> node, params object[] messages) where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"{node.OwnerName()}:", messages);
    }

    public static string OwnerName<T>(this IDebuggable<T> node) where T : Node
    {
        return ((T)node).Name;
    }

    public static string OwnerFileName<T>(this IDebuggable<T> node) where T : Node
    {
        return ((T)node).Filename;
    }

    public static void PrintCaller<T>(this IDebuggable<T> node, [CallerMemberName] string callerName = "")
        where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"{callerName} called ");
    }

    public static void PrintHasSignal<T>(this IDebuggable<T> node, string signal) where T : Node
    {
        if (node.IsDebugPrintEnabled()) GD.Print($"HasSignal({signal}): ", ((Node)node).HasSignal(signal));
    }

    public static void PrintSignalsList<T>(this IDebuggable<T> node, string signal) where T : Node
    {
        if (node.IsDebugPrintEnabled())
        {
            var signals = ((Node)node).GetSignalList();
            for (var i = 0; i < signals.Count; i++) GD.Print(signals[i]);
        }
    }

    public static void PrintSignalsListDump<T>(this IDebuggable<T> node, string signal) where T : Node
    {
        if (node.IsDebugPrintEnabled())
        {
            var signals = ((Node)node).GetSignalList();
            var result = $"{((Node)node).Name} : Signals\r\n=========";
            for (var i = 0; i < signals.Count; i++) result += $"-{signals[i]}\r\n";
            result += "=========";
            GD.Print(result);
        }
    }
}