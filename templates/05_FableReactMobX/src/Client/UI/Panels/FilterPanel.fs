module FilterPanel
open Feliz
open Model
open TextCtrl

[<ReactComponent>]
let FilterPanel (model: Model) (dispatch: Msg -> Unit) =
    let text, setText = React.useState ""
    
    React.useEffect (fun () -> printfn $"txt<-'{text}'"), [ text ]
    
    TextCtrl text setText


