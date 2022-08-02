module WebLit.AppContext

open ElmishStore

type Model = 
    {
        Username: string
    }

let init () = 
    { Username = "Anonymous" }, Cmd.none

type Msg = 
    | SetUsername of string

let update (msg: Msg) (model: Model) =
    match msg with
    | SetUsername username -> 
        { model with Username = username }, Cmd.none

let dispose (model: Model) = 
    printfn "Disposing"

let store, dispatch = Store.makeElmish init update dispose ()
