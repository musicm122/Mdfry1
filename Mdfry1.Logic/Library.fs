namespace Mdfry1.Logic

open Common.Interfaces
open Common.Manager
open Common.Services

module ServicesImpl =
    let db = JsonItemDatabase("foo")
    let Services = {
        Logger = Log.Logger
        Pauser = failwith "todo"
        AudioManager = failwith "todo"
        DialogManager =  new DialogManager()
        Database = db
        InventoryManager = InventoryManager(db)
    }
       