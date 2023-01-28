open Shared
open Saturn
open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

let musicStore : IMusicStore = {
    AllAlbums = async {
        return [
            { Id = 3; Name = "First" }
            { Id = 5; Name = "Second" }
        ]
    }
}


// create the HttpHandler from the musicStore value
let webApp : HttpHandler = 
    Remoting.createApi()
    |> Remoting.withRouteBuilder routeBuilder
    |> Remoting.fromValue musicStore
    |> Remoting.buildHttpHandler


let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }


(*
let app = application {
    url "http://localhost:6500/"
    use_router webApp
}
*)

run app

(*
let app = application {
    use_router (text "Hello World from Saturn")
}

run app
*)
