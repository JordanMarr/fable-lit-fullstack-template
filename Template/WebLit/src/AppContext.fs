module AppContext

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

let private dispose (m: Model) = 
    printfn "Disposing context"
    
let context = WebLit.UseContextHook.makeElmishContext init update dispose
let store, dispatch = context ()
