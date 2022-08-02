module WebLit.UseStore

open Fable
open Lit
open System

type HookContext with
    member ctx.useStore<'Model>(store: IStore<'Model>) = 
        
        // Create placeholder refs with temp defaults (to be replaced with real values from store)
        let setModelRef = ref(fun (_: 'Model) -> ())
        let subscriptionRef = ref { new IDisposable with member _.Dispose() = () }
        
        // Ensure that store subscription is disposed when component is destroyed
        ctx.useEffectOnce(fun () -> subscriptionRef.Value)

        let model, setModel = 
            ctx.useState(fun () ->            

                printfn "Susbscribing..."
                let initialModel, subscription = 
                    Store.subscribeImmediate (fun newModel -> 
                        printfn "Setting model..."
                        setModelRef.Value newModel
                    ) store 

                // Assign subscription ref (to be disposed with component)                
                //subscriptionRef.Value <- subscription
                subscriptionRef.Value <- 
                    { new IDisposable with 
                        member _.Dispose() = 
                            printf "Disposing subscription" 
                            subscription.Dispose()
                    }

                initialModel
            )

        setModelRef.Value <- setModel
        model

type Hook with
    static member inline useStore<'Model>(store: IStore<'Model>) = 
        Hook.getContext().useStore<'Model>(store)