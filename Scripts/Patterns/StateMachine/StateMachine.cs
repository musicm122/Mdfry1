using System.Collections.Generic;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Scripts.Patterns.StateMachine;

public class StateMachine
{
    // The collection of named states.
    private readonly Dictionary<string, State> _states = new();
    private ILogger Logger { get; } = new GDLogger();

    // The state that we're currently in.

    public State CurrentState { get; private set; }

    // The state that we'll start in.
    private State InitialState { get; set; }

    public State AddState(State newState)
    {
        // If this is the first state, it will be our initial state
        if (_states.Count == 0) InitialState = newState;

        // Add it to the dictionary
        _states[newState.Name] = newState;
        return newState;
    }

    // Updates the current state.
    public void Update(float delta)
    {
        // If we don't have any states to use, log the error.
        if (_states.Count == 0 || InitialState == null)
        {
            Logger.Error("State machine has no states!");
            return;
        }

        // If we don't currently have a state, transition to the initial
        // state.
        if (CurrentState == null)
        {
            Logger.Warning("Current state is null. Transitioning to initial state");
            TransitionTo(InitialState);
        }

        // If the current state has an onFrame method, call it.
        CurrentState.OnFrame?.Invoke(delta);
    }

    // Transitions to the specified state.
    public void TransitionTo(State newState)
    {
        // Ensure we weren't passed null
        if (newState == null)
        {
            Logger.Error("Cannot transition to a null state!");
            return;
        }

        // If we have a current state and that state has an on exit method,
        // call it
        if (CurrentState?.OnExit != null) CurrentState.OnExit();

        Logger.Info($"Transitioning from '{CurrentState}' to '{newState}'");

        // This is now our current state
        CurrentState = newState;

        // If the new state has an on enter method, call it
        newState.OnEnter?.Invoke();
    }

    // Transitions to a named state.
    public void TransitionTo(string name)
    {
        if (!_states.ContainsKey(name))
        {
            Logger.Error($"State machine doesn't contain a state named {name}!");
            return;
        }

        // Find the state in the dictionary
        var newState = _states[name];

        // Transition to it
        TransitionTo(newState);
    }


    // Transitions to a named state.
    public void TransitionTo<T>(T enumVal) where T : struct, System.Enum
    {
        if (!_states.ContainsKey(enumVal.GetDescription()))
        {
            Logger.Error($"State machine doesn't contain a state named {enumVal.GetDescription()}!");
            return;
        }

        // Find the state in the dictionary
        var newState = _states[enumVal.GetDescription()];

        // Transition to it
        TransitionTo(newState);
    }
}