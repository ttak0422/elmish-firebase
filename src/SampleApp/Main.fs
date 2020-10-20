module SampleApp

open Elmish
open Elmish.React
open Feliz
open Elf.Porter

type Model = { PushText: string; IsSignedIn: bool }

type Msg =
    | SignIn
    | Push of string
    | Read of string
    | SignedIn of bool

[<Porter("./main.js")>]
let signIn: unit -> unit = jsPorter

[<Porter("./main.js")>]
let push: string -> unit = jsPorter

let read, readSub = Porter.create Read
let signedIn, signedInSub = Porter.create SignedIn

let view (model: Model) (dispatch: Dispatch<Msg>) =
    Html.div [ if model.IsSignedIn then
                   Html.input [ prop.type' "input"
                                prop.value model.PushText
                                prop.onChange (Push >> dispatch) ]
               else
                   Html.button [ prop.onClick (fun _ -> dispatch SignIn)
                                 prop.text "Google SignIn" ] ]

let init () =
    { PushText = ""; IsSignedIn = false }, Cmd.none

let update (msg: Msg) (model: Model): Model * Msg Cmd =
    match msg with
    | SignIn ->
        model, Cmd.port signIn ()
    | Push text ->
        model, Cmd.port push text
    | Read text ->
        let model' = { model with PushText = text }
        model', Cmd.none
    | SignedIn isSignedIn ->
        let model' = { model with IsSignedIn = isSignedIn }
        model', Cmd.none

let subscriptions (model: Model) =
    [ readSub
      signedInSub ]

Program.mkProgram init update view
|> Program.withSubscription subscriptions
|> Program.withReactBatched "app"
|> Program.run
