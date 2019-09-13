namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html

[<JavaScript>]
module AutoCompletePage =

    open Components.Forms.AutoCompleteInput
    open Components.Forms

    type private template = Templating.Template<"templates/Module.AutoCompleteWidget.html">
    
    let Main go =
        let config = 
            new AutoCompleteInput.AutoCompleteInputConfig<int64,int64>(
                Callback = FrontEnd.Server.ListOfFruits,
                DomID = "fruit",
                Placeholder = "options: apple,avocado,banana,blackberry,blueberry,cherry,pear,pineapple",
                MinLength = 0
            )
        let rvValue = Var.Create { Key = 0L; Value = "" }
        let widgetDoc = 
            AutoCompleteInput rvValue config

        let config2 = 
            new AutoCompleteInput.AutoCompleteInputConfig<int64,int64>(
                Callback = FrontEnd.Server.ListOfFruits2,
                DomID = "fruit2",
                Placeholder = "options: banana,blackberry,blueberry,cherry",
                MinLength = 0
            )
        let rvValue2 = Var.Create { Key = 0L; Value = "" }
        let widgetDoc2 = 
            AutoCompleteInput rvValue2 config2

        let widgets = 
            div [] [ widgetDoc; widgetDoc2 ]

        let vWidget1 = 
            View.Map 
                (fun v -> sprintf "Widget 1: {id: %s; value: %s}" (string v.Key) v.Value ) 
                rvValue.View 

        let vWidget2 = 
            View.Map
                (fun v -> sprintf "Widget 2: {id: %s; value: %s}" (string v.Key) v.Value) 
                rvValue2.View

        template()
            .Widget(widgets)
            .WidgetState1(vWidget1)
            .WidgetState2(vWidget2)
            .Doc()
