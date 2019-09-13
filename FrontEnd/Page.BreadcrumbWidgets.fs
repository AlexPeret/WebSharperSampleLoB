namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module BreadcrumbPage =

    open Components.Nav.Breadcrumb
    
    let Main go =
        let paths = 
            [
                {
                    Label = "Level 1" 
                    Action = Path "/breadcrumb"
                }
                {
                    Label = "Level 2" 
                    Action = Callback (fun () -> Console.Log "Level 2")
                }
            ]
        Breadcrumb paths 

