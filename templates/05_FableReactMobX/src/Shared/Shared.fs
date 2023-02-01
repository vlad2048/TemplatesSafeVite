module Shared
open Structs

let routeBuilder typeName methodName = $"/api/{typeName}/{methodName}"


type IVideoApi = {
    getBatches: unit -> Async<Batch[]>
    getVideos: Filter -> Async<VideoSet>
}
