module WebLit.UseContextHook

open Lit
open System

type UseContextStore<'Model>(store: Fable.IStore<'Model>, defaultModel: 'Model) =
    member val internal Current = defaultModel with get,set
    interface Fable.IStore<'Model> with
        member this.Dispose() = store.Dispose()
        member this.Update(f) = store.Update(f)
        member this.Subscribe(observer: IObserver<'Model>) = store.Subscribe(observer)

let makeElmishContext init update = 
    let dispose = fun _ -> printfn "store.Dispose()"
    let store, dispatch = Store.makeElmish init update dispose ()
    let defaultModel = init () |> fst
    let ucStore = new UseContextStore<'Model>(store, defaultModel)
    ucStore, dispatch

type HookContext with
    member ctx.useContext<'Model>(store: UseContextStore<'Model>) = 
        let model, setModel = ctx.useState(store.Current)
        ctx.useEffectOnce (fun () -> 
            store.Subscribe (fun m -> 
                store.Current <- m
                setModel m
            )
        )
        model

type Hook with
    static member inline useContext<'Model>(store: UseContextStore<'Model>) = 
        Hook.getContext().useContext<'Model>(store)
