namespace Mdfry1.Logic.Enemy

open Godot
open Mdfry1.Logic.Constants.States
open Mdfry1.Logic.Sight

type SecurityCamera() =
    inherit Node2D()
    member val VisionManager:IVision option = None with get,set
    member val DebugLabel:Label option = None with get,set
    member val CameraSprite:Polygon2D option = None with get,set    
    member val CurrentCoolDownCounter = 0f with get,set
    member val MaxCoolDownTime = 0f with get,set
    member val IsStartMovement = true with get,set
    member val IsPausing = false with get,set
    member val Elapsed = 0f with get,set
    member val CurrentState= CameraState.Idle with get,set    
    
    override this._Ready() =
        this.DebugLabel <- Some (this.GetNode<Label>("DebugLabel"));
        this.VisionManager <- Some(this.GetNode<RaycastVision>("Pivot/RayCast2D"))
        // CameraSprite = GetNode<Polygon2D>("Polygon2D");
        // if (VisionManager == null) return;
        // VisionManager.OnTargetSeen += OnTargetDetection;
        // VisionManager.OnTargetOutOfSight += OnTargetLost;
