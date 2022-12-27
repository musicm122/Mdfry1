using System;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Scripts.Patterns.StateMachine;

// A single state.
[Serializable]
public class State
{
    // The state's visible name. Also used to identify the state to the
    // state machine.
    public string Name;

    // Called when the state is transitioned to from another state.
    public Action OnEnter;

    // Called when the state is transitioning to another state.
    public Action OnExit;

    // Called every frame while the state is active.
    public Action<float> OnFrame;
    public ILogger Logger { get; set; } = new GDLogger();

    public override string ToString()
    {
        return Name;
    }
}