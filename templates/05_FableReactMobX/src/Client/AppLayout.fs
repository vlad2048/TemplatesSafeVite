module AppLayout
open Elmish
open Fable.Remoting.Client
open Shared
open Feliz
open Model
open FlexLayout
open FilterPanel
open type Tag


let clsRoot = Cls "clsRoot"
let clsVideoRoot = Cls "clsVideoRoot"
let clsFilter = Cls "clsFilter"
let clsSideRoot = Cls "clsSideRoot"
let clsVideoBrowser = Cls "clsVideoBrowser"
let clsToggler = Cls "clsToggler"
let clsExtraFilterPanel = Cls "clsExtraFilterPanel"
let clsSourcePanel = Cls "clsSourcePanel"
let clsVideoPreviewer = Cls "clsVideoPreviewer"

let ui = BuildUI <|
    flexV (Flt, Flt)                                        clsRoot [
        flexV (Flt, Flt)                                    clsVideoRoot [
            flexH (Flt, Auto)                               clsFilter []
            flexH (Flt, Flt)                                clsSideRoot [
                scroll (Flt, Flt)                           clsVideoBrowser [
                    overlayV (Flt, Auto) TopLeft            clsExtraFilterPanel []
                    overlayScrollV (Auto, Flt) TopRight     clsSourcePanel []
                ]
                flexV (Fix 70, Flt)                         clsToggler []
            ]
        ]
        flexH (Flt, Fix 120)                                clsVideoPreviewer []
    ]
    

[<ReactComponent>]
let AppLayout (model: Model) (dispatch: Msg -> Unit) =
    ui
    |> With clsFilter [ FilterPanel model dispatch ]
    |> RenderUI
