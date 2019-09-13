namespace FrontEnd.Webparts

open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client

module LeftMenu =

    module Server =

        open FrontEnd.Config.Route

        type private template = Templating.Template<"templates/Module.LeftMenu.html">

        let private IconI iconClass fw =
            let fwClass = if fw then "fa-fw" else ""
            let css = sprintf "fa %s %s" iconClass fwClass
            i [ attr.``class`` css ] []

        let private IconSpan iconClass =
            let css = sprintf "fa %s" iconClass
            span [ attr.``class`` css ] []

        let private convertToItem (ctx: Context<EndPoint>) item = 
            let endpoint,name = item
            li [] 
               [ a [ attr.href (ctx.Link endpoint) ] 
                   [ text name ] ]
            
        (* Renders a menu item with optional subitem:
            <li><a href="..."><i class="fa fa-dashboard fa-fw"></i> level 1</a>
                <!-- optional sublist -->
                <ul>
                    <li><a href="..."> level 2</a>
            </li>
        
        *)
        let private MenuItem (ctx: Context<EndPoint>) icon name endpoint (subItems: (EndPoint*string) list) =
            let iconElem = IconI icon true
            let aElem = 
                a [ attr.href (ctx.Link endpoint) ]
                  [ iconElem
                    text name
                  ]

            let subItemsLi =
                subItems
                |> List.map (fun item -> convertToItem ctx item)

            let subItemsUl = 
                match subItemsLi with
                | [] -> Doc.Empty
                | items -> 
                    ul []
                       subItemsLi
                   
            li [] 
               [ aElem
                 subItemsUl
               ]
            
     
        let Run (ctx: Context<EndPoint>) =
            let widgetItems =
                [ (EndPoint.Datepicker,"Datepicker")
                  (EndPoint.AutoComplete,"Autocomplete")
                  (EndPoint.SelectBox,"SelectBox")
                  (EndPoint.Breadcrumb,"Breadcrumb")
                  (EndPoint.Tab,"Tab")
                  (EndPoint.GoogleMaps,"GoogleMaps")
                  (EndPoint.SlideShow,"SlideShow")
                  (EndPoint.Alert,"Alert")
                  (EndPoint.LoadingGif,"Loading Gif")
                  (EndPoint.ProgressBar,"ProgressBar")
                  (EndPoint.Attachments,"Attachments")
                  (EndPoint.DateTimeJS,"DateTime JS")
                ]
            
            let defaultLocation =
                async {
                    return! FrontEnd.Server.GetDefaultLocation()
                }
                |> Async.RunSynchronously

            let location = fst defaultLocation

            let samplePageItems =
                [ 
                  (EndPoint.Login,"Login")
                  (EndPoint.Restricted,"Restricted")
                  (EndPoint.ListingWithFilters (location,None,None,None,None), "Listing with Filters")
                  (EndPoint.ListingWithActions,"Listing with Actions")
                  (EndPoint.ClosedForm 1L,"Closed Form")
                  (EndPoint.OpenForm 1L,"Open Form")
                ]

            let items =
                [
                    MenuItem ctx "fa-dashboard" "Home" EndPoint.Home []
                    MenuItem ctx "fa-edit" "Widgets" EndPoint.Widgets widgetItems
                    MenuItem ctx "fa-edit" "Sample Pages" EndPoint.SamplePages samplePageItems
                    MenuItem ctx "fa-wrench" "Help" EndPoint.Help []
                ]

            template()
                .MenuItems(items)
                .Doc()
