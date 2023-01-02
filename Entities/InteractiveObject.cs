using Core.Input;
using Godot;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities;

public class InteractiveObject : Node2D, IDebuggable<Node>
{
    public bool CanInteract { get; set; }

    public bool IsInteracting { get; set; }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }


    public void OnEntered(Node body)
    {
        CanInteract = body.IsPlayer();
    }

    public void OnExited(Node body)
    {
        CanInteract = !body.IsPlayer();
    }

    public virtual void OnInteract()
    {
        if (!IsInteracting)
        {
            IsInteracting = true;
            GD.Print("Interacting with thing");
            OnInteractComplete();
        }
    }

    public virtual void OnInteractComplete()
    {
        IsInteracting = false;
    }

    public override void _Process(float delta)
    {
        if (CanInteract && PlayerActions.isInteracting()) OnInteract();
    }
}