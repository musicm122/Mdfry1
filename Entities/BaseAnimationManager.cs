using Godot;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities;

public class BaseAnimationManager : Node
{
    protected readonly ILogger _logger = new GDLogger(LogLevelOutput.Warning);
    protected AnimationTree AnimationTree { get; set; }
    protected AnimationPlayer BlinkAnimationPlayer { get; set; }
    protected AnimationNodeStateMachinePlayback StateMachinePlayback { get; set; }

    public virtual void UpdateAnimationBlendPositions(Vector2 movementVector)
    {
        _logger.Debug("UpdateAnimationBlendPositions arg:" + movementVector.ToString());
        UpdateAnimationBlendPosition("Idle", movementVector);
        UpdateAnimationBlendPosition("Walk", movementVector);
     
    }

    protected void UpdateAnimationBlendPosition(string animationName, Vector2 movementVector)
    {
        AnimationTree.Set($"parameters/{animationName}/blend_position", movementVector);
    }

    public Vector2 GetFacingDirection()
        => (Vector2)AnimationTree.Get($"parameters/Idle/blend_position");
    

    protected void NavToAnimation(string animationName)
    {
        StateMachinePlayback.Travel(animationName);
    }

    public void StartBlinkAnimation()
    {
        BlinkAnimationPlayer.Play("Start");
    }

    public void StopBlinkAnimation()
    {
        BlinkAnimationPlayer.Play("Stop");
    }
}