module WebApi.Views

open Giraffe.ViewEngine

let goodbye =
    html [] [
        head [] [
            title [] [ str "Signed Out" ]
        ]
        body [] [
            h1 [] [ str "Signed Out" ]
            p [] [ str "You have been signed out." ]
            p [] [
                a [ _href "/signin" ] [ str "Sign in" ]
            ]

        ]
    ]
