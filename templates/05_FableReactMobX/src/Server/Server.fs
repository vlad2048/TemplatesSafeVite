open Saturn
open Giraffe
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Shared
open FilterLogic

let private videos = Loader.LoadAllVideosDbg

let private videoApi: IVideoApi = {
    getBatches = fun _ -> async {
        let batches = Loader.LoadBatches()
        printfn $"BATCHES:{batches.Length}"
        return batches
    }
    getVideos = fun filter -> async {
        let! vids = videos
        let filteredVids = Filter filter vids
        printfn $"FILTERED_VIDEOS:{filteredVids.Videos.Length} ({filteredVids.TotalCount})"
        return filteredVids
    }
}


let webApp : HttpHandler = 
    Remoting.createApi()
    |> Remoting.withRouteBuilder routeBuilder
    |> Remoting.fromValue videoApi
    |> Remoting.buildHttpHandler


let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
