namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.JavaScript

[<JavaScript>]
module AlertPage =

    open Components.Display.Alert

    let Main go =
        let rvName = Var.Create ""

        let rvAlert = Var.Create (Success "")
        let alertElem1 = Alert rvAlert

        let rvAlert2 = Var.Create (Ok "")
        let alertElem2 = AlertResult rvAlert2

        let cnt = 
            [ form 
                []
                [ alertElem1
                  alertElem2
                  label [] [ text "name" ]
                  Doc.Input [] rvName
                  Doc.Button "send" [] (fun () -> 
                    if rvName.Value = "" then
                        rvAlert.Value <- Danger [ "name is required" ]
                        rvAlert2.Value <- Result.Error [ "name is required" ]
                    else
                        rvAlert.Value <- Success "data saved"
                        rvAlert2.Value <- Result.Ok "data saved"
                  )
                ]
            ]

        div [] cnt

           
