﻿using System;
using System.Threading.Tasks;
using Core.Input;
using Godot;
using Mdfry1.addons.dialogic.Other;
using Mdfry1.Logic.Constants;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Scripts.Item;

public class Examinable : Node2D, IDebuggable<Node>
{
    [Signal]
    public delegate void PlayerInteracting(Examinable examinable);

    [Signal]
    public delegate void PlayerInteractingAvailable(Examinable examinable);

    [Signal]
    public delegate void PlayerInteractingComplete(Examinable examinable);

    [Signal]
    public delegate void PlayerInteractingUnavailable(Examinable examinable);

    private const string Flashlight = "Flashlight";

    [Export] public string Timeline { get; set; }

    protected Area2D InteractableArea { get; set; }

    public bool CanInteract { get; set; }

    public bool ShouldRemove { get; set; }

    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public virtual void OnDialogListener(string val)
    {
    }

    public virtual void DialogListener(object value)
    {
        this.Print($"DialogListener called with arg {value}");
        this.Pause();
        var val = value.ToString();
        switch (val)
        {
            case Flashlight:
                GetTree().AddItem(Flashlight);
                ShouldRemove = true;
                break;
            case "Gun":
                GetTree().AddItem("Gun");
                ShouldRemove = true;
                break;
            case "HealthKitFound":
                GetTree().AddItem("HealthKit");
                ShouldRemove = true;
                break;
            case "AmmoFound":
            case "AmmoFound1":
                GetTree().AddItem("Ammo");
                ShouldRemove = true;
                break;
            case "AmmoFound5":
                GetTree().AddItem("Ammo", 5);
                ShouldRemove = true;
                break;
            case "AmmoFound10":
                GetTree().AddItem("Ammo", 10);
                ShouldRemove = true;
                break;
            case "AmmoFound20":
                GetTree().AddItem("Ammo", 20);
                ShouldRemove = true;
                break;
            case "LighterFluidFound":
                GetTree().AddItem("LampFluid");
                ShouldRemove = true;
                break;
            case "KeyA":
                this.Print("DialogListener: KEY switch");
                GetTree().AddItem("KeyA");
                ShouldRemove = true;
                break;
            case "Find the glasses":
                this.Print("adding 'Find the glasses' mission");
                GetTree().AddMission(val);
                break;
            default:
                OnDialogListener(val);
                break;
        }

        Task.Run(async () => await DialogComplete().ConfigureAwait(false));
    }

    private async Task DialogComplete()
    {
        this.Print($"Examinable.{nameof(DialogComplete)} called");
        this.Print($"Examinable.ShouldRemove = {ShouldRemove}");
        EmitSignal(nameof(PlayerInteractingComplete), this);
        await this.WaitForSeconds(0.2f).ConfigureAwait(false);
        this.Unpause();
        CanInteract = true;

        if (ShouldRemove) RemoveItem();
    }

    public virtual void StartDialog(string timeLine)
    {
        this.Print($"{nameof(Examinable)} StartDialog(${timeLine}) called");
        EmitSignal(nameof(PlayerInteracting), this);
        CanInteract = false;
        var dialog = DialogicSharp.Start(timeLine);
        var result = dialog.Connect("dialogic_signal", this, "DialogListener");
        if (result == Error.Ok)
            AddChild(dialog);
        else
            this.PrintError(result, $"{nameof(Examinable)} StartDialog(${timeLine}) failed");
    }

    protected virtual void OnInteract()
    {
        if (!Timeline.IsNullOrWhiteSpace())
            StartDialog(Timeline);
        else
            this.Print("No timeline assigned to Examinable");
    }

    private void RemoveItem()
    {
        this.Unpause();
        if (HasSignal(nameof(PlayerInteractingUnavailable))) EmitSignal(nameof(PlayerInteractingAvailable), this);
        Visible = false;
        SetProcess(false);
        SetPhysicsProcess(false);
        SetProcessInput(false);

        if (GetChildCount() > 0 && InteractableArea != null) RemoveChild(InteractableArea);
    }

    public virtual void OnExaminableAreaEntered(Node body)
    {
        if (body.IsPlayer() && HasSignal(nameof(PlayerInteractingAvailable)))
            EmitSignal(nameof(PlayerInteractingAvailable), this);
    }

    public virtual void OnExaminableAreaExited(Node body)
    {
        if (body.IsPlayer() && HasSignal(nameof(PlayerInteractingUnavailable)))
            EmitSignal(nameof(PlayerInteractingUnavailable), this);
    }

    private void ProcessLoop(float delta)
    {
        if (CanInteract && PlayerActions.isInteracting()) OnInteract();
    }

    public override void _Process(float delta)
    {
        ProcessLoop(delta);
    }

    private void RegisterInteractable(Area2D area2D)
    {
        area2D.TryConnectSignal(Signals.Area2D.BodyEntered, this, nameof(OnExaminableAreaEntered));
        area2D.TryConnectSignal(Signals.Area2D.BodyExited, this, nameof(OnExaminableAreaExited));
    }

    public override void _Ready()
    {
        AddToGroup(Groups.Examinable);
        InteractableArea = GetNode<Area2D>("Area2D");
        if (InteractableArea != null)
            RegisterInteractable(InteractableArea);
        else
            throw new NullReferenceException("Missing required child 'Area2D' for examinable");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup(Groups.Examinable);

    }
}