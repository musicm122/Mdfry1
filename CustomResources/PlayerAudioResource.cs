using Godot;

namespace Mdfry1.CustomResources;

public class PlayerAudioResource : EntityAudioResource
{
    [Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")]
    public string ShootClipPath { get; set; } = "";

    [Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")]
    public string EmptyClipPath { get; set; } = "";

    [Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")]
    public string DashClipPath { get; set; } = "";
}