module ColorUtils
open System


[<Literal>]
let private MaxHue = 360.0
let private hexChars = [| "0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "A"; "B"; "C"; "D"; "E"; "F" |]


type ColorUtils =
    static member MakePalette (count: int, ?ofs: int, ?seed: int, ?psat: float, ?pval: float): (unit -> string) =
        let colors =
            let psatV = Option.defaultValue 0.72 psat
            let pvalV = Option.defaultValue 0.58 pval
            let rnd = ColorUtils.MakeRnd seed
            let start = rnd.NextDouble() * MaxHue
            ColorUtils.SplitHueInterval count start
            |> List.map (fun phueV -> ColorUtils.ColorFromHSV (phueV, psatV, pvalV))
            |> Seq.toArray
        let ofsV = Option.defaultValue 0 ofs
        let mutable idx = ofsV % count
        fun () ->
            let idxPrev = idx
            idx <- (idx + 1) % count
            colors[idxPrev]
        
    static member private SplitHueInterval (count: int) (start: double) =
        [0..(count - 1)]
        |> List.map (fun i -> start + float i * (MaxHue / float count))
        |> List.map ColorUtils.NormalizeHue
    
    static member private NormalizeHue (hue: float) =
        let mutable res = hue
        while (res > MaxHue) do
            res <- res - MaxHue
        res
    
    static member private ColorFromHSV (phue: float, psat: float, pval: float) =
        let hi = (int (floor (phue / 60.0))) % 6
        let f = phue / 60.0 - floor(phue / 60.0)
        let pvalM = pval * 255.0
        let v = int pvalM
        let p = int (pvalM * (1.0 - psat))
        let q = int (pvalM * (1.0 - f * psat))
        let t = int (pvalM * (1.0 - (1.0 - f) * psat))
        let r, g, b =
            match hi with
            | 0 -> (v, t, p)
            | 1 -> (q, v, p)
            | 2 -> (p, v, t)
            | 3 -> (p, q, v)
            | 4 -> (t, p, v)
            | _ -> (v, p, q)
        $"#{ColorUtils.hex r}{ColorUtils.hex g}{ColorUtils.hex b}"

    static member private MakeRnd (seed: int Option): Random =
        match seed with
        | Some v -> Random(v)
        | None -> Random(int DateTime.Now.Ticks)

    static member private hex (v: int): string =
        hexChars[(v >>> 4) % 16] + hexChars[v % 16]
