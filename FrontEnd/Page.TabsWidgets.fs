namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.JavaScript

[<JavaScript>]
module TabPage =

    open Components.Nav.Tab
    
    let Main go =
        let tabs = 
            [
                {
                    Label = "Tab 1" 
                    Action = Path "#tab1"
                    IsActive = true
                }
                {
                    Label = "Tab 2" 
                    Action = Path "/tab"
                    IsActive = false
                }
                {
                    Label = "Tab 3" 
                    Action = Callback (fun () -> Console.Log "Tab 3")
                    IsActive = false
                }
                {
                    Label = "Tab 4" 
                    Action = Path "#tab4"
                    IsActive = false
                }
            ]
        
        let tabsElem = Tab tabs 
        let divTab1 = 
            div [ attr.id "tab1"
                  attr.``class`` "tab-pane fade in active"
                  Attr.Create "role" "tabpanel"
                  Attr.Create "aria-labelledby" "tab 1"
                ]
                [ text "tab1 content here" ]

        let divTab4 = 
            div [ attr.id "tab4"
                  attr.``class`` "tab-pane fade"
                  Attr.Create "role" "tabpanel"
                  Attr.Create "aria-labelledby" "tab 4"
                ]
                [ text "tab4 content here" ]

        let divTabContents =
            div [ attr.``class`` "tab-content" ]
                [ divTab1; divTab4 ]

        Doc.Concat [ tabsElem; divTabContents ]

