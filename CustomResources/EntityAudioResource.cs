using Godot;

namespace Mdfry1.CustomResources;

[Tool]
public class EntityAudioResource : Resource
{
    [Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")]
    public string TakeDamageClipPath { get; set; } = "";

    [Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")]
    public string DeathClipPath { get; set; } = "";

    [Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")]
    public string AttackClipPath { get; set; } = "";
}