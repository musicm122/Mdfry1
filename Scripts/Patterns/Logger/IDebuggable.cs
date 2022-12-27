using Godot;

namespace Mdfry1.Scripts.Patterns.Logger;

public interface IDebuggable<T> where T : Node
{
    [Export] bool IsDebugging { get; set; }

    bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }
}