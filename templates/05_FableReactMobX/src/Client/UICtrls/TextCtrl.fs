module TextCtrl
open Browser.Types
open Feliz

[<ReactComponent>]
let TextCtrl (text: string) (setText: string -> unit) =
    Html.input [
        prop.value text
        prop.onInput (fun evt -> setText (evt.target :?> HTMLInputElement).value)
    ]
