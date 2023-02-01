module App
open Shared
open Fable.Remoting.Client

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
