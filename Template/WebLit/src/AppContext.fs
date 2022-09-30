module WebLit.AppContext

open ElmishStore
open Ctrls

module Cmd = 
    let ofMsg (msg: 'Msg) = 
        Cmd.OfFunc.result msg

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
        Toast.success $"Name changed to {username}."
        { model with Username = username }, Cmd.none

let dispose _ = ()

let store, dispatch = Store.makeElmish init update dispose ()
