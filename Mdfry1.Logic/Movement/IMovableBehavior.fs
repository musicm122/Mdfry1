module Mdfry1.Logic.Movement

open Godot

type IMovableBehavior =
    abstract member OnMove: System.Action<Node2D,float32> with get, set
    abstract member OnIdle: System.Action<Node2D,float32> with get, set

    abstract member MaxSpeed:float32 with get, set 
    abstract member PushSpeed:float32  with get, set
    abstract member MoveMultiplier:float32  with get, set
    abstract member Velocity:Vector2  with get, set
    abstract member CanMove:bool with get, set
    abstract member IsRunning:bool with get, set
        
    abstract member HandleMovableObstacleCollision: Vector2 -> unit    
    abstract member GetMovementVelocityFromVelocity: Vector2 -> float32 -> Vector2
    abstract member GetMovementVelocityFromExistingMovementAndVelocity: Vector2 -> Vector2 -> float32 -> Vector2
    abstract member GetAcceleration: Vector2 -> Vector2 -> float32 -> Vector2
    abstract member GetFriction : Vector2 -> float32 -> Vector2
    abstract member Move: float32 -> unit
    

type MovableBehavior()=
    inherit KinematicBody2D()
    
    interface IMovableBehavior with

        member val CanMove = false with get, set
        member val IsRunning = false with get, set        
        member val MaxSpeed = 0f with get, set
        member val PushSpeed = 0f with get, set        
        member val MoveMultiplier = 0f with get, set
        member val Velocity = Vector2.Zero with get,set 
        member this.OnIdle = failwith "todo"
        member this.OnIdle with set value = failwith "todo"
        member this.OnMove = failwith "todo"
        member this.OnMove with set value = failwith "todo"
        member this.GetAcceleration(var0) (var1) (var2) = failwith "todo"
        member this.GetFriction(var0) (var1) = failwith "todo"
        member this.GetMovementVelocityFromExistingMovementAndVelocity(var0) (var1) (var2) = failwith "todo"
        member this.GetMovementVelocityFromVelocity(var0) (var1) = failwith "todo"
        member this.HandleMovableObstacleCollision(var0) = failwith "todo"
        member this.Move(var0) = failwith "todo"

    