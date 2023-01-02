namespace Core.Input

open Common.Extensions
open Godot

module InputConstants =

    [<Literal>]
    let Shoot = "shoot"

    [<Literal>]
    let Interact = "interact"

    [<Literal>]
    let Left = "left"

    [<Literal>]
    let Right = "right"

    [<Literal>]
    let Up = "up"

    [<Literal>]
    let Down = "down"

    [<Literal>]
    let Pause = "pause"

    [<Literal>]
    let Run = "run"

    [<Literal>]
    let Roll = "roll"

    [<Literal>]
    let ToggleFlashlight = "ToggleFlashlight"

    [<Literal>]
    let CameraUp = "camera_up"

    [<Literal>]
    let CameraDown = "camera_down"

    [<Literal>]
    let CameraLeft = "camera_left"

    [<Literal>]
    let CameraRight = "camera_right"

    [<Literal>]
    let CameraReset = "camera_reset"


    let AllInputs =
        [| Shoot
           Interact
           Left
           Right
           Up
           Down
           Pause
           Run
           Roll
           ToggleFlashlight
           CameraUp
           CameraDown
           CameraLeft
           CameraRight
           CameraReset |]

module PlayerActions =
    let isShooting () =
        Input.IsActionJustPressed InputConstants.Shoot

    let isInteracting () =
        Input.IsActionJustPressed InputConstants.Interact

    let isAnyKeyPressed () =
        InputConstants.AllInputs |> Array.exists Input.IsActionJustPressed

module CameraMovement =
    let getCameraMovementInput speed =
        let mutable velocity = Vector2.Zero

        if
            Input.IsActionPressed(InputConstants.CameraRight)
            && Input.IsActionPressed(InputConstants.CameraUp)
        then
            velocity <- velocity.AddToX 0.5f
            velocity <- velocity.SubFromY 0.5f

        if
            Input.IsActionPressed(InputConstants.CameraRight)
            && Input.IsActionPressed(InputConstants.CameraDown)
        then
            velocity <- velocity.AddToX 0.5f
            velocity <- velocity.AddToY 0.5f

        if
            Input.IsActionPressed(InputConstants.CameraLeft)
            && Input.IsActionPressed(InputConstants.CameraUp)
        then
            velocity <- velocity.SubFromX 0.5f
            velocity <- velocity.SubFromY 0.5f

        if
            Input.IsActionPressed(InputConstants.CameraLeft)
            && Input.IsActionPressed(InputConstants.CameraDown)
        then
            velocity <- velocity.SubFromX 0.5f
            velocity <- velocity.AddToY 0.5f

        if Input.IsActionPressed(InputConstants.CameraRight) then
            velocity <- velocity.AddToX 1f

        if Input.IsActionPressed(InputConstants.CameraLeft) then
            velocity <- velocity.SubFromX 1f

        if Input.IsActionPressed(InputConstants.CameraUp) then
            velocity <- velocity.SubFromY 1f

        if Input.IsActionPressed(InputConstants.CameraDown) then
            velocity <- velocity.AddToY 1f

        velocity.Normalized() * speed

    let isCameraReset () =
        Input.IsActionJustPressed InputConstants.CameraReset
