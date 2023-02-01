module FlexLayout
open ColorUtils
open FSharp.Core
open System
open Browser
open Browser.Types
open Feliz
open Fable.React




// *********
// * Trees *
// *********
type Nod<'a> = {
    Val: 'a
    Kids: Nod<'a> list
}

type Tree =
    static member map (f: 'a -> 'b) (node: Nod<'a>): Nod<'b> = {
        Val = f node.Val
        Kids = node.Kids |> List.map (Tree.map f)
    }
    static member toList (nod: Nod<'a>): 'a list =
        nod.Val
        :: (
              nod.Kids
              |> List.map Tree.toList
              |> List.concat
           )

let private flatten (nested: 'a list list): 'a list =
    seq {
        for arr in nested do
        for elt in arr do
        yield elt
    }
    |> Seq.toList
    
    

// **********
// * Inputs *   Lay
// **********
type Cls = Cls of string

type Dir = Horz | Vert

type Dim =
      Auto
    | Fix of int
    | Flt

type OverlayAnchor =
      TopLeft
    | TopRight
    | BottomLeft
    | BottomRight
    
type LayNode = {
    Cls: Cls
    DimX: Dim
    DimY: Dim
    Dir: Dir
    Scroll: bool
    Overlay: OverlayAnchor Option
}


type Tag =
    static member inline flexH (dimX: Dim, dimY: Dim) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Horz (dimX, dimY) None false cls kids

    static member inline flexV (dimX: Dim, dimY: Dim) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Vert (dimX, dimY) None false cls kids

    static member inline scroll (dimX: Dim, dimY: Dim) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Vert (dimX, dimY) None true cls kids

    static member inline overlayH (dimX: Dim, dimY: Dim) (anchor: OverlayAnchor) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Horz (dimX, dimY) (Some anchor) false cls kids
    static member inline overlayV (dimX: Dim, dimY: Dim) (anchor: OverlayAnchor) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Vert (dimX, dimY) (Some anchor) false cls kids

    static member inline overlayScrollH (dimX: Dim, dimY: Dim) (anchor: OverlayAnchor) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Horz (dimX, dimY) (Some anchor) true cls kids
    static member inline overlayScrollV (dimX: Dim, dimY: Dim) (anchor: OverlayAnchor) (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> =
        Tag.mk Vert (dimX, dimY) (Some anchor) true cls kids


    static member inline private mk
        (dir: Dir) (dimX: Dim, dimY: Dim)
        (anchor: OverlayAnchor option) (scroll: bool)
        (cls: Cls) (kids: Nod<LayNode> list): Nod<LayNode> = {
            Val = {
                Cls = cls
                Dir = dir
                DimX = dimX
                DimY = dimY
                Scroll = scroll
                Overlay = anchor
            }
            Kids = kids
        }




// ****************************
// * Intermediate Description *     Css
// ****************************
type private CssDisplay =
    CssDisplayFlex of Dir               // display: flex; flex-direction: row / column

type private CssDim =                   // width / height
      CssDimAuto                        // auto                     (default)
    | CssDimFix of int                  // Xpx
    | CssDimFill                        // 100%

//                  grow    shrink  basis
// default: flex    0       1       auto
// auto     -> size according to its content
type private CssFlex =                  // flex
      CssFlexFix of int                 // 0 0 X     =  X
    | CssFlexAuto                       // 0 0 auto  =  auto
    | CssFlexFill                       // 1 1 auto  _

type private CssOverflow =              // overflow-y
      CssOverflowNone                   // visible                  (default)
    | CssOverflowClip                   // hidden
    | CssOverflowScroll                 // auto

type private CssPosition =
      CssPositionNormal                 // static
    | CssPositionOverlay                // absolute
    | CssPositionOverlayParent          // relative

type private CssOverlayPos = OverlayAnchor

type private CssAttr =
      CssAttrDisplay of CssDisplay
    | CssAttrDim of (Dir * CssDim)
    | CssAttrFlex of CssFlex
    | CssAttrOverflow of CssOverflow
    | CssAttrPosition of CssPosition
    | CssAttrOverlayPos of CssOverlayPos

type private CssNode =
    { Cls: Cls
      Attrs: CssAttr list }


// ***************
// * CSS classes *      Text
// ***************
type private CssTextAttr = CssTextAttr of string

type private CssClass = {
    Cls: Cls
    Attrs: CssTextAttr list
}


// ******
// * UI *
// ******
type UI =
    { Root: Nod<Cls>
      Map: Map<Cls, ReactElement list> }

let With (cls: Cls) (kids: ReactElement list) (ui: UI): UI =
    { ui with Map = ui.Map.Add(cls, kids) }


// ***************************************
// * Convert to intermediate description *  Lay -> Css
// ***************************************
let private ResolveDir (dir: Dir): CssAttr list = [
    CssAttrDisplay (CssDisplayFlex dir)
]

let private ResolveDim (dir: Dir) (isDir: bool) (dim: Dim): CssAttr list =
    match isDir with
        | true -> match dim with
                        Dim.Auto  -> [ CssAttrFlex CssFlexAuto ]
                      | Dim.Fix v -> [ CssAttrFlex (CssFlexFix v) ]
                      | Dim.Flt   -> [ CssAttrFlex CssFlexFill; CssAttrDim (dir, CssDimFill) ]
        | false -> match dim with
                        Dim.Auto  -> [ CssAttrDim (dir, CssDimAuto) ]
                      | Dim.Fix v -> [ CssAttrDim (dir, (CssDimFix v)) ]
                      | Dim.Flt   -> [ CssAttrDim (dir, CssDimFill) ]


let private ResolveScroll (scroll: bool): CssAttr list =
    match scroll with
    | false -> [ CssAttrOverflow CssOverflowClip ]
    | true -> [ CssAttrOverflow CssOverflowScroll ]

let private ResolveOverlay (isOverlay: OverlayAnchor Option) (hasAnyOverlayChildren: bool): CssAttr list =
    match (isOverlay, hasAnyOverlayChildren) with
        | None    , false -> []
        | Some pos, _     -> [ CssAttrPosition CssPositionOverlay; CssAttrOverlayPos pos ]
        | None    , true  -> [ CssAttrPosition CssPositionOverlayParent ]
    

let rec private Resolve (parentDir: Dir Option) (node: Nod<LayNode>): Nod<CssNode> =
    let hasAnyOverlayChildren = List.exists (fun c -> c.Val.Overlay.IsSome) node.Kids
    let n = node.Val

    {
        Val = {
            Cls = n.Cls
            Attrs =
                flatten [
                ResolveDim Horz (Option.contains Horz parentDir) n.DimX
                ResolveDim Vert (Option.contains Vert parentDir) n.DimY
                ResolveDir n.Dir
                ResolveScroll n.Scroll
                ResolveOverlay n.Overlay hasAnyOverlayChildren
            ]
        }
        Kids = node.Kids |> List.map (Resolve (Some node.Val.Dir))
    }

let private Lay2Css (root: Nod<LayNode>): Nod<CssNode> =
    Resolve None root


// **************************
// * Convert to CSS classes *   Css -> Cls
// **************************
let private ConvertAttr (attr: CssAttr): CssTextAttr list =
    match attr with
      | CssAttrDisplay (CssDisplayFlex Horz) -> [ "display: flex"; "flex-direction: row" ]
      | CssAttrDisplay (CssDisplayFlex Vert) -> [ "display: flex"; "flex-direction: column" ]

      | CssAttrDim (Horz, CssDimAuto ) -> [ "width: auto" ]
      | CssAttrDim (Vert, CssDimAuto ) -> [ "height: auto" ]
      | CssAttrDim (Horz, CssDimFix v) -> [ $"width: {v}px" ]
      | CssAttrDim (Vert, CssDimFix v) -> [ $"height: {v}px" ]
      | CssAttrDim (Horz, CssDimFill ) -> [ "width: 100%" ]
      | CssAttrDim (Vert, CssDimFill ) -> [ "height: 100%" ]

      | CssAttrFlex (CssFlexFix v)     -> [ $"flex: 0 0 {v}px" ]
      | CssAttrFlex CssFlexAuto        -> [ "flex: 0 0 auto" ]
      | CssAttrFlex CssFlexFill        -> [ "flex: 1 1 auto" ]
      
      | CssAttrOverflow CssOverflowNone   -> [ "overflow-y: visible" ]
      | CssAttrOverflow CssOverflowClip   -> [ "overflow-y: hidden" ]
      | CssAttrOverflow CssOverflowScroll -> [ "overflow-y: auto" ]

      | CssAttrPosition CssPositionNormal        -> [ (*"position: static"*) ]
      | CssAttrPosition CssPositionOverlay       -> [ "position: absolute" ]
      | CssAttrPosition CssPositionOverlayParent -> [ "position: relative" ]

      | CssAttrOverlayPos TopLeft     -> [ "left: 0"; "top: 0" ]
      | CssAttrOverlayPos TopRight    -> [ "right: 0"; "top: 0" ]
      | CssAttrOverlayPos BottomLeft  -> [ "left: 0"; "bottom: 0" ]
      | CssAttrOverlayPos BottomRight -> [ "right: 0"; "bottom: 0" ]
    |> List.map CssTextAttr

let private Css2Cls (palette: unit -> string) (root: Nod<CssNode>): Nod<CssClass> =
    root
    |> Tree.map(fun css -> {
        Cls = css.Cls
        Attrs =
            CssTextAttr $"background-color: {palette()}"
            :: (css.Attrs |> List.map ConvertAttr |> flatten)
    })



let private BuildClsDef (nfo: CssClass) =
    let indentedLines = List.map (fun e -> $"    {e};") (nfo.Attrs |> List.map(function CssTextAttr str -> str))
    let unwrap (Cls v) = v
    let clsName = unwrap nfo.Cls
    $".{clsName} {{\n" + String.Join("\n", indentedLines) + "\n}"


// ***********
// * BuildUI *
// ***********
let BuildUI (rootLay: Nod<LayNode>): UI =
    let palette = ColorUtils.MakePalette (10, 0, 0)
    let clsTree =
        rootLay
        |> Lay2Css
        |> Css2Cls palette
    let cssContent = String.Join("\n\n", clsTree |> Tree.map BuildClsDef |> Tree.toList)
    let head = (document.getElementsByTagName "head")[0] :?> HTMLHeadElement
    let node = document.createElement "style" :?> HTMLStyleElement
    node.textContent <- cssContent
    head.appendChild node |> ignore
    {
        Root = clsTree |> Tree.map (fun e -> e.Cls)
        Map = Map.empty
    }


// ************
// * RenderUI *
// ************
let RenderUI (ui: UI): ReactElement =
    let rec renderNode (node: Nod<Cls>): ReactElement =
        let (Cls clsName) = node.Val
        let treeKids = node.Kids |> List.map renderNode
        let dynaKids = defaultArg (ui.Map.TryFind node.Val) []
        
        Html.div [
            prop.className clsName
            treeKids @ dynaKids |> prop.children
        ]
        
    renderNode ui.Root
