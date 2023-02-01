module Structs
open System


type Batch = {
    Site: string
    Name: string
}


type Video = {
    Batch: Batch
    Url: string
    Name: string
    ImgUrl: string
    TimeAdded: DateTime
    Description: string option
    Duration: TimeSpan option
    ViewCount: int option
}


type Filter =
    { Batches: Batch[]
      SearchText: string
      DurationMin: int option
      DurationMax: int option
      PageIndex: int
      PageSize: int }
    static member empty: Filter = {
        Batches = [||]
        SearchText = ""
        DurationMin = None
        DurationMax = None
        PageIndex = 0
        PageSize = 15
    }
    static member Batches_     = (fun e -> e.Batches    ), (fun v e -> { e with Batches     = v })
    static member SearchText_  = (fun e -> e.SearchText ), (fun v e -> { e with SearchText  = v })
    static member DurationMin_ = (fun e -> e.DurationMin), (fun v e -> { e with DurationMin = v })
    static member DurationMax_ = (fun e -> e.DurationMax), (fun v e -> { e with DurationMax = v })
    static member PageIndex_   = (fun e -> e.PageIndex  ), (fun v e -> { e with PageIndex = v   })

type VideoSet = {
    Videos: Video[]
    TotalCount: int
}


[<Literal>]
let VideosJsonSample =
    """
    [
        {
            "BatchUrl": "http://www.asianfemdoms.com/videos/all-recent.html",
            "Url": "http://www.asianfemdoms.com/video/x1-fwagXbYi1p9.html",
            "Name": "x1-fwagXbYi1p9.html",
            "ImgUrl": "http://www.asianfemdoms.com/images/videos/0797/3958/main.jpg",
            "TimeAdded": "2022-12-18T11:27:25.1963163+00:00",
            "Description": "\u3010\u4E5D\u6708\u5DE8\u732E\u3011\u5A9A\u513F\u5973\u738B\uFFFD...",
            "Duration": "00:10:14",
            "ViewCount": 0
        },
        {
            "BatchUrl": "http://www.asianfemdoms.com/videos/all-recent.html",
            "Url": "http://www.asianfemdoms.com/video/x1-fwagXbYi1p9.html",
            "Name": "x1-fwagXbYi1p9.html",
            "ImgUrl": "http://www.asianfemdoms.com/images/videos/0797/3958/main.jpg",
            "TimeAdded": "2022-12-18T11:27:25.1963163+00:00"
        }
    ]
    """
