namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client

[<JavaScript>]
module SelectBoxPage =

    open Components.Forms.SelectBoxInput

    type private template = Templating.Template<"templates/Module.SelectBoxWidget.html">

    let private mapOption (rv:Var<SelectItem<int>>) =
        rv.View
        |> View.Map (fun (item) -> 
            item.Value
        )           

    let private mapToSelectItem (key,value) =
        { Key = key; Value = value }

    (* Setup the selectbox control with an initial list of options
       and then replaces such list with a remote call. 
    *)
    let private EagerAndLazyLoadTest() =
        let options = 
            [
                mapToSelectItem (1,"Option1")
                mapToSelectItem (2,"Option2")
            ]
        let option = options.Item 1

        let rvOption = Var.Create option
        let rvListOptions = Var.Create options
        let elem = 
            SelectBoxInput "" rvOption rvListOptions
        
        async {
            let! options2' = 
                FrontEnd.Server.ListOfOptions ()
            let options2 =
                options2'
                |> List.map mapToSelectItem
        
            Var.Set rvListOptions <| options2 
            Var.Set rvOption <| options2.Item 1
        }
        |> Async.Start
        
        let vOption = mapOption rvOption

        let widgetTemplate1 = 
            template.WidgetTemplate()
                .Widget(elem)
                .WidgetState1(vOption)
                .Doc()

        widgetTemplate1
    
    (* Lazy load scenario using Doc.Async *)
    let private LazyLoadTest() =
        let initialOption = { Key = 0; Value = "" }
        let rvOption2 = Var.Create initialOption
        let rvListOptions2 = Var.Create []
        let elem2 = 
            SelectBoxInput "" rvOption2 rvListOptions2
        
        let vOption2 = mapOption rvOption2

        let widgetTemplate2 = 
            async {
                let! options2' = 
                    FrontEnd.Server.ListOfOptions ()
                let options2 = 
                    options2'
                    |> List.map mapToSelectItem
        
                Var.Set rvListOptions2 options2
                Var.Set rvOption2 <| options2.Item 1

                return 
                    template.WidgetTemplate()
                        .Widget(elem2)
                        .WidgetState1(vOption2)
                        .Doc()
            }
            |> Doc.Async

        widgetTemplate2

    let Main go =

        (* Note: by concatenating both components at the bottom of this function,
                 WebSharper.UI will wait until both are done to render them.
        *)
        let widgetTemplate1 = EagerAndLazyLoadTest()

        let widgetTemplate2 = LazyLoadTest()

        (* Uncomment the line below to test the eager load scenario from Test1() *)
        //let widgetTemplate2 = Doc.Empty

        template()
            .Content2([widgetTemplate1;widgetTemplate2])
            .Doc()
        
