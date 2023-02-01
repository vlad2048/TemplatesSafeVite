module App
open Elmish
open Elmish.React
open Shared
open Fable.Remoting.Client
open Feliz
open Model
open AppLayout
#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif



let init () : Model * Cmd<Msg> =
    { Dummy = "dummy" }, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
      | Dumb s -> { model with Dummy = s }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit): ReactElement =
    AppLayout model dispatch
    


Program.mkProgram init update view
//#if DEBUG
|> Program.withConsoleTrace
//#endif
|> Program.withReactSynchronous "elmish-app"
//#if DEBUG
|> Program.withDebugger
//#endif
|> Program.run



(*
printfn "Starting"

let musicStore : IMusicStore = 
  Remoting.createApi()
  |> Remoting.withRouteBuilder routeBuilder
  |> Remoting.buildProxy<IMusicStore>

async {
    let! albums = musicStore.AllAlbums
    for album in albums do
        printfn "%A (%A)" album.Id album.Name
}
|> Async.StartImmediate
*)

