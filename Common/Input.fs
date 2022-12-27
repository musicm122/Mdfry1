namespace Core.Input

    module InputAction = 

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


        let AllInputs = [|
            Shoot
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
            CameraReset
        |]