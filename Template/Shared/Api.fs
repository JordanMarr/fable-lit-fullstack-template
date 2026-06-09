namespace Shared.Api

open Shared.Validation
open Serde.FS

type CatFact = { Fact: string }
type CatFactPage = { Current_Page: int; Data: CatFact list }
type PageSize = int
type PageNumber = int

type CatInfo = 
    {
        Name: string
        Age: int
        LastVetCheckup: System.DateTime
    }
    member this.Validate() = 
        rules
        |> rulesFor (nameof this.Name) [ 
            this.Name |> Rules.required
            this.Name |> Rules.maxLen 10
        ]
        |> rulesFor (nameof this.Age) [
            Rules.isTrue (this.Age > 0) "Age must be a positive number."
        ]
        |> rulesFor (nameof this.LastVetCheckup) [
            // A custom rule
            let timeSinceLastVetCheckup = System.DateTime.Today - this.LastVetCheckup.Date
            printfn $"Total days since last checkup: {timeSinceLastVetCheckup.TotalDays}"
            if this.Age >= 10 && timeSinceLastVetCheckup.TotalDays > 90 then 
                Error "Cats over 10 years old should get a vet checkup every three months."
            elif timeSinceLastVetCheckup.TotalDays > 180 then 
                Error "Cats under 10 years old should get a vet checkup every six months."
            else 
                Ok ()
        ]
        |> validate


/// A type that specifies the communication protocol between client and server.
/// This [<RpcApi>] interface drives code generation on both ends:
/// - The server dispatch/codecs are generated in the WebApi project (via Serde.FS.Json.AspNet).
/// - The typed Fable client proxy is generated in the WebLit project's fable-generated/
///   folder, automatically, because WebLit references the Serde.FS.Json.Fable package.
/// To learn more, read the docs at https://github.com/serde-fs/Serde.FS
[<RpcApi>]
type IServerApi =
    abstract member GetCatFacts: PageSize * PageNumber -> Async<CatFact list>
