module App
open Shared
open Fable.Remoting.Client
open Browser
open Feliz.JSX.Solid
open Fable.Core

printfn "Starting"

let musicStore : IMusicStore = 
  Remoting.createApi()
  |> Remoting.withRouteBuilder routeBuilder
  |> Remoting.buildProxy<IMusicStore>

async {
    let! albums = musicStore.AllAlbums
    for album in albums do
        printfn $"{album.Id} ({album.Name})"
}
|> Async.StartImmediate


[<JSX.Component>]
let App() =
    let num, setNum = Solid.createSignal(7)
    Html.div [
        Html.div $"Solid Main {num()}"
        Html.button [
            Ev.onClick(fun _ -> num() + 1 |> setNum)
            Html.children [ Html.text "Increment" ]
        ]
    ]

Solid.render((fun () -> App()), document.getElementById("app-container"))
