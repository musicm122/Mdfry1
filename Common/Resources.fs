namespace Common.Resources

open Godot

type EntityAudioResourceFS() = 
    inherit Resource()
    [<Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")>]
    member val DashClipPath = ""
    
    [<Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")>]
    member val EmptyClipPath = ""
    
type PlayerAudioResourceFS() = 
    inherit EntityAudioResourceFS()
    
    [<Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")>]
    member val DeathClipPath = ""
    
    [<Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")>]
    member val TakeDamageClipPath = ""
    
    [<Export(PropertyHint.File, "*.wav, *.ogg, *.mp3")>]
    member val AttackClipPath = ""
    
    

