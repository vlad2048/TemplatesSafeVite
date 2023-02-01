module FilterLogic
open Structs
open System

let Filter (filter: Filter) (videos: Video list): VideoSet =
    let textParts =
        filter.SearchText
            .Split(' ', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

    let matchBatches (vid: Video) =
        filter.Batches
        |> Array.contains vid.Batch
    
    let matchText (vid: Video) =
        let contains (a: string) (b: string) = a.Contains(b, StringComparison.InvariantCultureIgnoreCase)
        textParts
        |> Array.forall (fun part ->
            (contains vid.Name part) ||
            (vid.Description |> (Option.exists (fun descr -> contains descr part)))
        )
    
    let matchDuration (vid: Video) =
        match vid.Duration with
          | None -> false
          | Some time ->
              let d = int time.TotalMinutes
              match (filter.DurationMin, filter.DurationMax) with
                | None, None -> true
                | Some min, None -> d >= min
                | None, Some max -> d <= max
                | Some min, Some max -> d >= min || d <= max
    
    let filterFun (vid: Video): bool =
        (matchBatches vid) ||
        (matchText vid) ||
        (matchDuration vid)
        
    let filteredVideos =
        videos
        |> List.filter filterFun
    
    let pageVideos =
        filteredVideos
        |> List.skip (filter.PageSize * filter.PageIndex)
        |> List.take filter.PageIndex
        |> Seq.toArray

    {
        Videos = pageVideos
        TotalCount = filteredVideos.Length
    }