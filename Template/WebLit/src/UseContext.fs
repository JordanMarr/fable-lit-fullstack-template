module WebLit.UseContext

open Lit
open System

type UseContextStore<'Model>(store: Fable.IStore<'Model>, defaultModel: 'Model) =
    member val internal Current = defaultModel with get,set
    interface Fable.IStore<'Model> with
        member this.Dispose() = store.Dispose()
        member this.Update(f) = store.Update(f)
        member this.Subscribe(observer: IObserver<'Model>) = store.Subscribe(observer)

let makeElmishContext init update props = 
    let dispose = fun _ -> 
        //printfn "Disposing useContext subscription"
        ()

    let store, dispatch = Store.makeElmish init update dispose props
    let defaultModel = init props |> fst
    let ucStore = new UseContextStore<'Model>(store, defaultModel)
    ucStore, dispatch

type HookContext with
    member ctx.useContext<'Model>(store: UseContextStore<'Model>) = 
        let model, setModel = ctx.useState(store.Current)
        
        ctx.useEffectOnce (fun () -> 
            //printfn "Subscribing..."
            let subscription = 
                store.Subscribe (fun m -> 
                    store.Current <- m
                    setModel m
                )

            // dispose subscription when component is destroyed
            subscription
        )
        model

type Hook with
    static member inline useContext<'Model>(store: UseContextStore<'Model>) = 
        Hook.getContext().useContext<'Model>(store)
