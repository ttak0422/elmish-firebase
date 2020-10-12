module SampleApp

open Elmish
open Feliz


type Model = { Count: int }

type Msg = Incr

let view model dispatch =
    Html.div [ Html.h1 [ prop.text (string model.Count) ]
               Html.button [ prop.text "Incr"
                             prop.onClick (fun _ -> dispatch Incr) ] ]

let init () = { Count = 0 }, Cmd.none

let update msg model =
    match msg with
    | Incr -> { model with Count = model.Count + 1 }, Cmd.none

open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update view
|> Program.withDebugger
|> Program.withReactBatched "app"
|> Program.run
