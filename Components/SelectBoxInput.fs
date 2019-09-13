namespace Components.Forms

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
module SelectBoxInput = 
    type SelectItem<'a> = {
        Key: 'a
        Value: string
    }

    let SelectBoxInputC cssClass callback (rvSelectedOption:Var<SelectItem<'a>>) (rvOptions:Var<SelectItem<'a> list>) = 
        let vLstOptions = rvOptions.View

        let cssClass' = 
            if System.String.IsNullOrEmpty(cssClass) then
                "form-control"
            else
                cssClass

        let elem = 
            vLstOptions
            |> View.Map (fun _ ->
                let selectBox =
                    Doc.SelectDyn 
                        [ attr.``class`` cssClass'; on.change (fun elem evt -> callback (rvSelectedOption.Value)) ] 
                        (fun (item) -> item.Value) vLstOptions rvSelectedOption
                selectBox
                )
            |> Doc.EmbedView
        elem

    let SelectBoxInput cssClass (rvSelectedOption:Var<SelectItem<'a>>) (rvOptions:Var<SelectItem<'a> list>) = 
        let callback _ = ()
        SelectBoxInputC cssClass callback rvSelectedOption rvOptions
