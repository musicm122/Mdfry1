namespace Mdfry1.Logic.Sight

open Godot
open Common
open Common.Extensions

type SightState =
    | Blocked
    | CanSee
    | TooFarAway

type IVision =
    abstract member OnTargetSeen: (Node2D -> unit)
    abstract member OnTargetOutOfSight: (Node2D -> unit)
    abstract member CanCheckFrames: int -> bool
    abstract member UpdateFacingDirection: Vector2 -> unit
    abstract member LookAtPoint: Vector2 -> unit
    abstract member CanSeeTarget: Node2D -> SightState
    abstract member OldTarget: Node2D option with get, set
    abstract member NewTarget: Node2D option with get, set

type Area2dVision() =
    inherit Area2D()
    //Some(this.Vision.OnTargetSeen) |> Option.iter (fun onSeen -> onSeen (newTarget))

    [<Export>]
    member val private Targets: string[] = [| "player" |] with get, set

    member val private LineOfSight: bool = false with get, set

    member this.Vision
        with private get (): IVision = (this :> IVision)

    member this.TargetInLineOfSight(body: Node2D) : bool =
        this.Targets |> Array.contains (body.Name.Trim().ToLowerInvariant())
        && this.HasLineOfSight(body.Position)

    member private this.OnVisionRadiusBodyEntered(node: Node) =
        let hasLineOfSight = this.TargetInLineOfSight(node :?> Node2D)

        if hasLineOfSight then
            this.LineOfSight <- true
            let newTarget = (node :?> Node2D)
            this.Vision.NewTarget <- Some newTarget
            Some(this.Vision.OnTargetSeen) |> Option.iter (fun onSeen -> onSeen (newTarget))
            this.Vision.OldTarget <- None

    member private this.OnVisionRadiusBodyExit(body: Node) =
        match this.Vision.OldTarget with
        | Some oldTarget ->
            let isTarget = this.Targets |> Array.contains (body.Name.Trim().ToLowerInvariant())
            let node = body :?> Node2D
            // Not sure this exit code worked before since old code passed in global position for exit and local position for entrance.
            let hasLineOfSight = this.TargetInLineOfSight(oldTarget)

            if isTarget && not hasLineOfSight then
                Some this.Vision.OnTargetOutOfSight
                |> Option.iter (fun onOutOfSight -> onOutOfSight (oldTarget))
        | _ -> ()

    override this._Ready() =
        match this.ConnectBodyEntered this (nameof (this.OnVisionRadiusBodyEntered)) with
        | err when err <> Error.Ok -> failwithf "Signal: OnVisionRadiusBodyEntered failed to be connected"
        | _ -> ()

        match this.ConnectBodyExited this (nameof (this.OnVisionRadiusBodyExit)) with
        | err when err <> Error.Ok -> failwithf "Signal: OnVisionRadiusBodyExit failed to be connected"
        | _ -> ()


    interface IVision with
        member this.CanCheckFrames(var0) = failwith "todo"
        member this.CanSeeTarget(var0) = failwith "todo"
        member this.LookAtPoint(var0) = failwith "todo"
        member this.NewTarget = failwith "todo"

        member this.NewTarget
            with set value = failwith "todo"

        member this.OldTarget = failwith "todo"

        member this.OldTarget
            with set value = failwith "todo"

        member this.OnTargetOutOfSight = failwith "todo"
        member this.OnTargetSeen = failwith "todo"
        member this.UpdateFacingDirection(var0) = failwith "todo"

type RaycastVision() =
    inherit RayCast2D()

    interface IVision with
        member this.CanCheckFrames(var0) = failwith "todo"
        member this.CanSeeTarget(var0) = failwith "todo"
        member this.LookAtPoint(var0) = failwith "todo"
        member this.NewTarget = failwith "todo"

        member this.NewTarget
            with set value = failwith "todo"

        member this.OldTarget = failwith "todo"

        member this.OldTarget
            with set value = failwith "todo"

        member this.OnTargetOutOfSight = failwith "todo"
        member this.OnTargetSeen = failwith "todo"
        member this.UpdateFacingDirection(var0) = failwith "todo"
