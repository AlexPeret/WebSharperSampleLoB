namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Html
open WebSharper.Google.Maps

[<JavaScript>]
module GoogleMapsPage =

    open FrontEnd.Config
    
    let Main go =
        let afterRenderAttr =
            on.afterRender(fun elem -> 
                let center = new LatLng (37.4419, -122.1419)
                let options = new MapOptions (center, 8)
                let map = new Google.Maps.Map(elem,options)

                let move () = map.PanTo(new LatLng(37.4569, -122.1569))
                JS.SetTimeout move 5000 |> ignore

                let markerOptions = new MarkerOptions(center)
                markerOptions.Map <- map
                new Marker(markerOptions) |> ignore
            )

        div [ attr.style "padding-bottom:20px; width:500px; height:300px;"
              afterRenderAttr ]
            []
