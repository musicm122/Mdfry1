using Godot;

namespace Mdfry1.Scripts.Patterns.Logger
{
    public interface IDebuggable<T> where T : Node
    {
        bool IsDebugPrintEnabled() => IsDebugging;
        
        
        [Export]
        bool IsDebugging { get; set; }
    }
}