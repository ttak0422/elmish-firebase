module SampleApp

open System
open Elmish
open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop
open Feliz

[<RequireQualifiedAccessAttribute>]
module Sub =
    let batch (subs: Sub<'msg> list): Cmd<'msg> = subs

type Model = { PushText: string; IsSignedIn: bool }

type Msg =
    | SignIn
    | Push of string
    | Read of string
    | SignedIn of bool

module Port =
    module Internal =
        type CallJsFromFs =
            abstract signIn: unit -> unit
            abstract push: string -> unit

        [<ImportAll("./main.js")>]
        let callJsFromFs: CallJsFromFs = jsNative

        let readEvent = Event<string>()
        let signedInEvent = Event<bool>()
        let readObserver = readEvent.Publish
        let signedInObserver = signedInEvent.Publish

    let signIn = Internal.callJsFromFs.signIn

    let push = Internal.callJsFromFs.push

    let readSub (toMsg: string -> Msg): Sub<Msg> =
        fun (dispatch: Dispatch<Msg>) ->
            Internal.readObserver
            |> Observable.subscribe (toMsg >> dispatch)
            |> ignore

    let signedInSub (toMsg: bool -> Msg): Sub<Msg> =
        fun (dispatch: Dispatch<Msg>) ->
            Internal.signedInObserver
            |> Observable.subscribe (toMsg >> dispatch)
            |> ignore

let read = Port.Internal.readEvent.Trigger
let signedIn = Port.Internal.signedInEvent.Trigger


let view (model: Model) (dispatch: Dispatch<Msg>) =
    Html.div [ if model.IsSignedIn then
                   Html.input [ prop.type' "input"
                                prop.value model.PushText
                                prop.onInput (fun e -> !!e.target?value |> Push |> dispatch) ]
               else
                   Html.button [ prop.onClick (fun _ -> dispatch SignIn)
                                 prop.text "Google SignIn" ] ]

let init () =
    { PushText = ""; IsSignedIn = false }, Cmd.none

let update (msg: Msg) (model: Model): Model * Msg Cmd =
    match msg with
    | SignIn ->
        Port.signIn ()
        model, []
    | Push text ->
        Port.push (text)
        model, []
    | Read text ->
        let model' = { model with PushText = text }
        model', Cmd.none
    | SignedIn isSignedIn ->
        let model' = { model with IsSignedIn = isSignedIn }
        model', Cmd.none

let subscriptions (model: Model) =
    Sub.batch [ Port.readSub Read
                Port.signedInSub SignedIn ]

open Elmish.HMR

Program.mkProgram init update view
|> Program.withSubscription subscriptions
|> Program.withReactBatched "app"
|> Program.run
