namespace FrontEnd.Pages.Widgets

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
module SelectBox =

    open Components.Forms.SelectBoxInput
    open FrontEnd.Config
    open Components.Forms

    type private template = Templating.Template<"templates\\Module.SelectBoxWidget.html">

    let private mapOption (rv:Var<int*string>) =
        rv.View
        |> View.Map (fun (_,value) -> 
            value
        )           

    //let Main () =
    //    let options = 
    //        [
    //            1,"Option1"
    //            2,"Option2"
    //        ]
    //    let option = options.Item 1

    //    let rvOption,rvListOptions,elem = 
    //        SelectBoxInput "" option options
        
    //    async {
    //        let! options2 = 
    //            FrontEnd.Server.ListOfOptions ()
        
    //        Var.Set rvListOptions options2
    //        Var.Set rvOption <| options2.Item 1
    //    }
    //    |> Async.Start
        
    //    let vOption = mapOption rvOption

    //    let widgetTemplate1 = 
    //        template.WidgetTemplate()
    //            .Widget(elem)
    //            .WidgetState1(vOption)
    //            .Doc()


    //    (* Now, setting the selectbox using Doc.Async *)
    //    let rvOption2,rvListOptions2,elem2 = 
    //        SelectBoxInput "" (0,"") []
        
    //    let vOption2 = mapOption rvOption2

    //    let widgetTemplate2 = 
    //        async {
    //            let! options2 = 
    //                FrontEnd.Server.ListOfOptions ()
        
    //            Var.Set rvListOptions2 options2
    //            Var.Set rvOption2 <| options2.Item 1

    //            return 
    //                template.WidgetTemplate()
    //                    .Widget(elem2)
    //                    .WidgetState1(vOption2)
    //                    .Doc()
    //        }
    //        |> Doc.Async

    //    template()
    //        .Content1(widgetTemplate1)
    //        //.Content2([widgetTemplate1;widgetTemplate2])
    //        .Content2(widgetTemplate2)
    //        .Doc()

    let private Test1() =
        let options = 
            [
                1,"Option1"
                2,"Option2"
            ]
        let option = options.Item 1

        let rvOption,rvListOptions,elem = 
            SelectBoxInput "" option options
        
        async {
            let! options2 = 
                FrontEnd.Server.ListOfOptions ()
        
            Var.Set rvListOptions options2
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
    
    let private Test2() =
        let rvOption2,rvListOptions2,elem2 = 
            SelectBoxInput "" (0,"") []
        
        let vOption2 = mapOption rvOption2

        let widgetTemplate2 = 
            async {
                let! options2 = 
                    FrontEnd.Server.ListOfOptions ()
        
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

    let Main () =

        let widgetTemplate1 =
            async {
                return Test1()
            }
            |> Doc.Async

        (* Now, setting the selectbox using Doc.Async *)
        let widgetTemplate2 = Test2()

        //template()
        //    .Content1(widgetTemplate1)
        //    //.Content2([widgetTemplate1;widgetTemplate2])
        //    .Content2(widgetTemplate2)
        //    .Doc()
        Doc.Append widgetTemplate1 widgetTemplate2

