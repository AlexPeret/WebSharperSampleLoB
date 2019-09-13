namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html

[<JavaScript>]
module AllWidgetsPage =

    open Components.Forms
    open FrontEnd.Config

    type private template = Templating.Template<"templates/Page.Widgets.html">

    let Main () =
        let pageContent = 
            div []
                [ text "list of all widgets..." ]

        template()
            .WidgetContent(pageContent)
            .Doc()
