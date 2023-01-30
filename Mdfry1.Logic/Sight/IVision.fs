namespace Mdfry1.Logic.Sight

open Godot
open Common

//todo: look at refactoring canSeeTarget to return  SightState instead of bool
type SightState =
    | Blocked = 0
    | Visible = 1
    | TooFarAway = 2
    | NoKnownTarget = 3

type IVision =
    abstract member OnTargetSeen: (Node2D -> unit) option with get, set
    abstract member OnTargetOutOfSight: (Node2D -> unit) option with get, set
    abstract member CanCheckFrames: int -> bool
    abstract member UpdateFacingDirection: Vector2 -> unit
    abstract member CanSeeTarget:unit -> bool
    
    abstract member OldTarget: Node2D option with get, set
    abstract member NewTarget: Node2D option with get, set

type RaycastVision() =
    inherit RayCast2D()

    member this.IsTrackedTarget(body: Node2D) =
        this.Targets |> Array.contains (body.Name.Trim().ToLowerInvariant())

    member val private LineOfSight: bool = false with get, set

    member val private CanSeeTarget = false with get, set

    [<Export>]
    member val StartDirection = Vector2.Up with get, set

    [<Export>]
    member val ConeAngle = Mathf.Deg2Rad(30f) with get, set

    [<Export>]
    member val AngleBetweenRays = Mathf.Deg2Rad(5f) with get, set

    [<Export>]
    member val MaxViewDistance = 100f with get, set

    [<Export>]
    member val FrameCheckInterval = 2 with get, set

    [<Export>]
    member val private Targets: string[] = [| "player" |] with get, set

    member this.Vision
        with private get (): IVision = (this :> IVision)

    member this.GenerateSightRays() =
        this.GenerateRayCasts this.ConeAngle this.AngleBetweenRays this.StartDirection this.MaxViewDistance

    member this.GetTargetCollider() : Node2D option =
        this.GetChildrenOfTypeAsArray<RayCast2D>()
        |> Array.map (fun (ray: RayCast2D) -> ray.GetCollider() :?> Node2D)
        |> Array.tryFind (this.IsTrackedTarget)

    member this.FlushVisionCheck() =
        let newTarget = this.GetTargetCollider()
        let oldTarget = this.Vision.OldTarget

        match (newTarget, oldTarget) with
        | Some newT, _ ->
            this.Vision.NewTarget <- Some newT
            this.CanSeeTarget <- true
            this.Vision.OnTargetSeen |> Option.map (fun onSeen -> onSeen newT) |> ignore
            ()
        | None, Some oldT ->
            this.CanSeeTarget <- false
            this.Vision.OnTargetOutOfSight
            |> Option.map (fun onOutOfSight -> onOutOfSight oldT)
            |> ignore

            ()
        | _ -> ()

        this.Vision.OldTarget <- newTarget
        this.Vision.NewTarget <- None

    override this._Ready() = this.GenerateSightRays()

    override this._PhysicsProcess _ =
        if not (this.Vision.CanCheckFrames this.FrameCheckInterval) then
            ()

        this.FlushVisionCheck()

    interface IVision with
        member this.CanCheckFrames(interval: int) =
            (System.Random().Next(0) % interval) = 0

        member this.CanSeeTarget() =
            this.CanSeeTarget
        member val NewTarget: Node2D option = None with get, set
        member val OldTarget: Node2D option = None with get, set
        member val OnTargetSeen: (Node2D -> unit) option = None with get, set
        member val OnTargetOutOfSight: (Node2D -> unit) option = None with get, set

        member this.UpdateFacingDirection newVelocity =
            this.Rotation <- this.Position.AngleToPoint(newVelocity.Normalized())
