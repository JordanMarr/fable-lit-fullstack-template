module Shared.Api
open Shared.Validation

/// Defines how routes are generated on server and mapped from client
let routerPaths typeName method = sprintf "/api/%s" method

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
/// To learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IServerApi = {
    GetCatFacts: PageSize * PageNumber -> Async<CatFact list>
}