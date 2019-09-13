namespace FrontEnd.Pages.Samples

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.JavaScript

// remover
open WebSharper.UI.Templating

[<JavaScript>]
module ListingWithActionsPage =

    open FrontEnd
    open Components.Display.Alert

    module Services = FrontEnd.Server

    type private template = Templating.Template<"templates/Page.ListingWithActions.html">

    let Main go =
        let rvStatusMsg = Var.Create (Success "")
        let statusMsg = Alert rvStatusMsg

        async {
            let! model =
                Services.DefaultFilterModel()

            let! properties =
                Services.FindProperties model

            let listItems =
                properties
                |> List.map (fun p ->
                    template.Item()
                        .Title(p.Title)
                        .Subtitle(p.Subtitle)
                        .Description(p.Description)
                        .Price(string p.Price)
                        .OnItemClick(fun t ->
                            Var.Set rvStatusMsg <| Success (p.Title)
                        )
                        .Doc()
                )
                
            return
                template()
                    .StatusMsg(statusMsg)
                    .Items(listItems)
                    .Doc()
        }
        |> Doc.Async
            
