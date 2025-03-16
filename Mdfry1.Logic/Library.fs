namespace Mdfry1.Logic

open Common.Database
open Common.Interfaces
open Common.Manager
open Common.Services

module ServicesImpl =
    let db = JsonItemDatabase("foo")
    let Services = {
        Logger = Log.Logger
        PauseManager = failwith "todo"
        AudioManager = failwith "todo"
        DialogManager =  new DialogManager()
        Database = db
        InventoryManager = InventoryManager(db)
    }
       