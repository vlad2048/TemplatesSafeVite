module Shared

open System

type Album = {
    Id: int
    Name: string
}

type IMusicStore = {
    AllAlbums: Async<Album list>
}

let routeBuilder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName
