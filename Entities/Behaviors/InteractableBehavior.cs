using System;
using System.Collections.Generic;
using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.Enum;
using Mdfry1.Scripts.Extensions;
using Mdfry1.Scripts.Item;
using Mdfry1.Scripts.Patterns.Logger;

namespace Mdfry1.Entities.Behaviors;

public class InteractableBehavior : Node2D, IDebuggable<Node>, IInteractableBehavior
{
    [Export] public bool IsDebugging { get; set; }

    public bool IsDebugPrintEnabled()
    {
        return IsDebugging;
    }

    public PlayerDataStore DataStore { get; set; }

    public Action<Examinable> InteractingCallback { get; set; }
    public Action<Examinable> InteractingCompleteCallback { get; set; }
    public Action<Examinable> InteractingAvailableCallback { get; set; }
    public Action<Examinable> InteractingUnavailableCallback { get; set; }

    public bool HasKey(Key key)
    {
        return DataStore.Inventory.HasKey(key) || key == Key.None;
    }

    public bool HasItem(string itemName)
    {
        return DataStore.Inventory.HasItem(itemName);
    }

    public Sprite ExaminableNotification { get; set; }
    public bool CanInteract { get; set; }

    public void RegisterExaminable(List<Examinable> examinableCollection)
    {
        this.Print("Examinable count = ", examinableCollection.Count);
        if (!examinableCollection.IsNullOrEmpty())
        {
            examinableCollection.ForEach(e =>
                e.Connect(nameof(Examinable.PlayerInteracting), this, nameof(OnInteractionBegin)));
            examinableCollection.ForEach(e =>
                e.Connect(nameof(Examinable.PlayerInteractingComplete), this, nameof(OnInteractingComplete)));
            examinableCollection.ForEach(e =>
                e.Connect(nameof(Examinable.PlayerInteractingAvailable), this, nameof(OnInteractionAvailable)));
            examinableCollection.ForEach(e =>
                e.Connect(nameof(Examinable.PlayerInteractingUnavailable), this, nameof(OnInteractionUnavailable)));
        }
    }

    public override void _Ready()
    {
        ExaminableNotification = GetNode<Sprite>("ExaminableNotification");
        RegisterExaminable(GetTree().GetExaminableCollection());
        RegisterLockedDoors(GetTree().GetLockedDoorCollection());
        ExaminableNotification.Hide();
    }

    public void ShowExamineNotification()
    {
        ExaminableNotification.Show();
    }

    public void HideExamineNotification()
    {
        ExaminableNotification.Hide();
    }

    public void OnInteractionAvailable(Examinable examinable)
    {
        this.PrintCaller();
        CanInteract = true;
        examinable.CanInteract = true;
        ShowExamineNotification();
    }

    public void OnInteractionUnavailable(Examinable examinable)
    {
        this.PrintCaller();
        CanInteract = false;
        examinable.CanInteract = false;
        HideExamineNotification();
    }

    public void OnInteractionBegin(Examinable examinable)
    {
        this.PrintCaller();
        examinable.CanInteract = false;
        CanInteract = false;
        InteractingCallback?.Invoke(examinable);
    }

    public void OnInteractingComplete(Examinable examinable)
    {
        examinable.CanInteract = true;
        CanInteract = true;
        InteractingCompleteCallback?.Invoke(examinable);
    }

    public void OnDoorInteraction(LockedDoor lockedDoor)
    {
        this.PrintCaller();
        this.Print($"Does player have key {lockedDoor.RequiredKey}", HasKey(lockedDoor.RequiredKey));
        this.Print($"lockedDoor.CurrentDoorState  = {lockedDoor.CurrentDoorState}");

        if (HasKey(lockedDoor.RequiredKey) && lockedDoor.CurrentDoorState == DoorState.Locked)
            lockedDoor.CurrentDoorState = DoorState.Closed;

        InteractingCallback?.Invoke(lockedDoor);
    }

    public void OnDoorInteractionComplete(LockedDoor lockedDoor)
    {
        InteractingCompleteCallback?.Invoke(lockedDoor);
    }

    public void Init(PlayerDataStore dataStore)
    {
        DataStore = dataStore;
    }

    private void RegisterLockedDoors(List<LockedDoor> lockedDoorCollection)
    {
        this.Print("lockedDoorCollection count = ", lockedDoorCollection.Count);
        if (!lockedDoorCollection.IsNullOrEmpty())
            lockedDoorCollection.ForEach(e =>
            {
                e.Connect(nameof(LockedDoor.DoorInteraction), this, nameof(OnDoorInteraction));
                e.Connect(nameof(LockedDoor.DoorInteractionComplete), this, nameof(OnDoorInteractionComplete));
            });
    }
}