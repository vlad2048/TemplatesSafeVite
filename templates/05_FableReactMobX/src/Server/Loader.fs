module Loader
open Structs
open System.IO
open FSharp.Data

let folder = @"C:\Dev\Creepy\_data - Copy"


let LoadBatches () =
    query {
        for dir in Directory.GetDirectories folder do
        for name in Directory.GetFiles dir do
        select {
            Site = Path.GetFileName dir;
            Name = Path.GetFileNameWithoutExtension name
        }
    }
    |> Seq.toArray

type Provider = JsonProvider<VideosJsonSample>

let LoadBatchVideos (batch: Batch) =
    async {
        let! text =
            Path.Combine(folder, batch.Site, batch.Name + ".json")
            |> File.ReadAllTextAsync
            |> Async.AwaitTask
        return
            text
            |> Provider.Parse
            |> Array.map (fun e -> {
                Batch = batch;
                Url = e.Url;
                Name = e.Name;
                ImgUrl = e.ImgUrl;
                TimeAdded = e.TimeAdded.DateTime;
                Description = e.Description;
                Duration = e.Duration;
                ViewCount = e.ViewCount;
            })
    }

(*let LoadBatchVideosSync (batch: Batch) =
    Path.Combine(folder, batch.Site, batch.Name + ".json")
    |> File.ReadAllText
    |> Provider.Parse
    |> Array.map (fun e -> {
        BatchUrl = e.BatchUrl;
        Url = e.Url;
        Name = e.Name;
        ImgUrl = e.ImgUrl;
        TimeAdded = e.TimeAdded.DateTime;
        Description = e.Description;
        Duration = e.Duration;
        ViewCount = e.ViewCount;
    })*)



let LoadAllVideos =
    async {
        let! seqs =
            LoadBatches()
                |> Seq.map LoadBatchVideos
                |> Async.Parallel
        return
            query {
                for arr in seqs do
                for vid in arr do
                select vid
            }
            |> Seq.toList
    }
    



let LoadAllVideosDbg =
    async {
        let! seqs =
            LoadBatches()
                |> Seq.take 1
                |> Seq.map LoadBatchVideos
                |> Async.Parallel
        return
            query {
                for arr in seqs do
                for vid in arr do
                select vid
            }
            |> Seq.toList
    }
