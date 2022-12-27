using Godot;

namespace Mdfry1.Scripts.GDUtils;

public static class DialogicUtils
{
    private static readonly Script _dialogic = GD.Load<Script>("res://addons/dialogic/Other/DialogicClass.gd");

    public static Node GetDialog(string timeLine)
    {
        var retval = _dialogic.Call("Start", timeLine);
        return (Node2D)retval;
    }
}