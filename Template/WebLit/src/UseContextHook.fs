module WebLit.UseContextHook

open Fable
open Lit
open System

type HookContext with
    member ctx.useStore<'Model>(store: IStore<'Model>) = 
        let setModel = ref(fun (_: 'Model) -> ())
        let dispose = ref { new IDisposable with member _.Dispose() = () }

        ctx.useEffectOnce(fun () -> dispose.Value)
        let model, setModel' = ctx.useState(fun () ->
            
            let value, dispose' = 
                Store.subscribeImmediate (fun newValue -> 
                    setModel.Value newValue
                ) store 
            dispose.Value <- dispose'
            value
        )
        setModel.Value <- setModel'
        model

type Hook with
    static member inline useStore<'Model>(store: IStore<'Model>) = 
        Hook.getContext().useStore<'Model>(store)