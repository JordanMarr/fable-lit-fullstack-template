module WebApi.Views

open Giraffe
open Giraffe.ViewEngine

let devMode =
    html [] [
        head [] [ title [] [ str "Dev Mode" ] ]
        body [] [
            h1 [] [
                str "Server Web Api - Dev Mode"
            ]
            p [] [
                a [ _href "http://localhost:3000" ] [
                    str "Fable: http://localhost:3000"
                ]
            ]
            p [] [
                str "To start Fable development web server, browse to Client folder and type 'npm start'."
            ]
            p [] [
                str "To deploy, 'npm run build', then 'Deploy' Server project to Azure target environment."
            ]
        ]
    ]
