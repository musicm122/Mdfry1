using Godot;
using System;
using Mdfry1.Logic.Constants;

public class LevelMap : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddToGroup(Groups.LevelMap);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveFromGroup(Groups.LevelMap);
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
