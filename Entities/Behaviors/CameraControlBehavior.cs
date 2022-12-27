using System.Globalization;
using Core.Input;
using Godot;
using Mdfry1.Entities.Behaviors.Interfaces;
using Mdfry1.Scripts.Constants;
using Mdfry1.Scripts.Patterns.Logger;
using Mdfry1.Scripts.Patterns.Logger.Implementation;

namespace Mdfry1.Entities.Behaviors;

public class CameraControlBehavior : Camera2D, IDebuggable<Node>, IMovableCamera
{
    private ILogger _logger;
    [Export] public float DefaultZoomLevel { get; set; } = 1.0f;
    [Export] public Vector2 DefaultPan { get; set; } = new(0, 0);
    [Export] public float MinZoom { get; set; } = 0.5f;
    [Export] public float MaxZoom { get; set; } = 2f;

    [Export] public float ZoomFactor { get; set; } = 0.1f;
    [Export] public float ZoomDuration { get; set; } = 0.2f;

    [Export] public float ZoomLevel { get; set; } = 1.0f;

    [Export] public Vector2 MaxPanRight { get; set; } = new(100, 0);
    [Export] public Vector2 MaxPanLeft { get; set; } = new(-100, 0);

    [Export] public Vector2 MaxPanUp { get; set; } = new(0, -100);

    [Export] public Vector2 MaxPanDown { get; set; } = new(0, 100);

    private Tween TweenUtil { get; set; }
    [Export] public bool IsDebugging { get; set; }

    public void SetZoom(float amount)
    {
        _logger.Debug("Setting zoom to: " + amount);
        const string zoomProperty = "zoom";
        ZoomLevel = Mathf.Clamp(amount, MinZoom, MaxZoom);
        var endZoomLevel = new Vector2(ZoomLevel, ZoomLevel);

        TweenUtil.InterpolateProperty(
            this,
            zoomProperty,
            Zoom,
            endZoomLevel,
            ZoomDuration,
            Tween.TransitionType.Sine,
            Tween.EaseType.Out
        );
        TweenUtil.Start();
    }

    public void SetPan(Vector2 newOffset)
    {
        _logger.Debug("Setting pan offset to: " + newOffset);
        const string offsetProperty = "offset";

        TweenUtil.InterpolateProperty(
            this,
            offsetProperty,
            Offset,
            newOffset,
            ZoomDuration,
            Tween.TransitionType.Sine,
            Tween.EaseType.Out
        );
        TweenUtil.Start();
    }

    public void ResetCamera()
    {
        _logger.Debug("Camera should be reset");
        SetZoom(DefaultZoomLevel);
        SetPan(DefaultPan);
    }

    public override void _Ready()
    {
        _logger = IsDebugging
            ? new GDLogger(LogLevelOutput.Debug)
            : new GDLogger(LogLevelOutput.Warning);
        TweenUtil = GetNode<Tween>("Tween");
        ResetCamera();
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        var upStrength = inputEvent.GetActionStrength(InputAction.CameraUp);
        var downStrength = inputEvent.GetActionStrength(InputAction.CameraDown);
        var leftStrength = inputEvent.GetActionStrength(InputAction.CameraLeft);
        var rightStrength = inputEvent.GetActionStrength(InputAction.CameraRight);


        _logger.Debug(
            @$"| downStrength = {downStrength.ToString(CultureInfo.InvariantCulture)} |
| upStrength= {upStrength.ToString(CultureInfo.InvariantCulture)} |
| leftStrength={leftStrength.ToString(CultureInfo.InvariantCulture)} | 
| rightStrength= {rightStrength.ToString(CultureInfo.InvariantCulture)} |");
        if (leftStrength > 0.2f)
        {
            var newOffset = MaxPanLeft * leftStrength;
            SetPan(newOffset);
        }

        if (rightStrength > 0.2f)
        {
            var newOffset = MaxPanRight * rightStrength;
            SetPan(newOffset);
        }

        if (upStrength > 0.2f)
        {
            var newOffset = MaxPanUp * upStrength;
            SetPan(newOffset);
        }

        if (downStrength > 0.2f)
        {
            var newOffset = MaxPanDown * downStrength;
            SetPan(newOffset);
        }

        if (inputEvent.IsActionPressed(InputAction.CameraReset)) ResetCamera();
    }
}