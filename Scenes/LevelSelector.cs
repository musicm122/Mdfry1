using Godot;
using Mdfry1.Scripts.Extensions;

namespace Mdfry1.Scenes;

public class LevelSelector : Node
{
    [Export(PropertyHint.File, "*.tscn")]
    public string PlayGameScene { get; set; } = "res://Scenes/Level/PrototypeMap.tscn";

    public override void _Ready()
    {
        var interaction = GetNode<Button>("RootControl/VBoxContainer/PlayGame");
        interaction.ConnectButtonPressed(this, nameof(OnInteractionButtonPressed));

        var quit = GetNode<Button>("RootControl/VBoxContainer/Quit");
        quit.ConnectButtonPressed(this, nameof(OnQuitButtonPressed));

        interaction.GrabFocus();
    }

    public void OnInteractionButtonPressed()
    {
        GetTree().ChangeScene(PlayGameScene);
    }


    public void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}