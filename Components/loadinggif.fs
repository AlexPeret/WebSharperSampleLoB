namespace Components.Display

open System
open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.Sitelets

open WebSharper.JavaScript

[<JavaScript>]
module LoadingGif = 

    let private Render (rShow:Var<bool>) css =
        let vClass = 
            rShow.View
            |> View.Map (fun show ->
                let css = 
                    if show then css
                    else "hidden"
                css
            )

        div
            [ attr.classDyn vClass ]
            [ div [ attr.``class`` "osahanloading" ]
                  []
            ]

    let LoadingGifFull (rShow:Var<bool>) =
        Render rShow "animationload-fullscreen"
          
    let LoadingGif (rShow:Var<bool>) =
        Render rShow "animationload"
          
    (* output:
        <div class="animationload">
        <div class="osahanloading"></div>
        </div>
    *)
