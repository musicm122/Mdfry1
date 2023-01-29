namespace Mdfry1.Logic.Constants

open System.Collections.Generic

module Groups =
    let Player = "Player"
    let Movable = "Movable"
    let AllEnemies = "Enemies"
    let Spawner = "Spawner"
    let Ammo = "Ammo"
    let Items = "Items"
    let LampFluids = "LampFluids"
        
module ItemConstants =
    let PizzaImagePath = "res://Assets/Art/Food/pizzaslice.png"
    let FlashlightImagePath = "res://Assets/Art/ItemDefinition/Flashlight.png"
    let HealthkitImagePath = "res://Assets/Art/ItemDefinition/genericItem_color_102.png"
    let KeyImagePath = "res://Assets/Art/ItemDefinition/genericItem_color_155.png"
    let LampFluidImagePath = "res://Assets/Art/ItemDefinition/genericItem_color_107.png"

    let ItemImagePaths =
        let paths = Dictionary<string,string>()
        paths.Add("Pizza", PizzaImagePath)
        paths.Add("Flashlight", FlashlightImagePath)
        paths.Add("KeyA", KeyImagePath)
        paths.Add("Healthkit", HealthkitImagePath)
        paths.Add("LampFluid", LampFluidImagePath)
        paths
        

module States =
    type CameraState =
        | Idle = 0
        | Warning = 1
        | Aggro = 2
        | Damaged =3
        | Stun = 4
        
    type DoorState =
        | Locked = 2
        | Closed = 1
        | Opened = 0
