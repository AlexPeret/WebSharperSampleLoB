namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.Swiper
open WebSharper.JQuery

[<JavaScript>]
module SlideShowPage =
    open FrontEnd.Config

    type private template = Templating.Template<"templates/Module.SlideShowSample.html">
    
    [<Direct "requestFullScreen($element)">]
    let FullScreen (element:Dom.Element) = X<unit>

    [<Require(typeof<UIResources.SlideShowSample.Css>)>]
    let Main go =
        let afterRenderAttr = 
            on.afterRender(fun _ ->
                let swipeParams = 
                    SwipeParameters(
                        SpaceBetween = 10,
                        NextButton = Union2Of2 ".swiper-button-next",
                        PrevButton = Union2Of2 ".swiper-button-prev",
                        Parallax = true,
                        Speed = 600,
                        //AutoHeight = true,
                        OnDoubleTap = (fun swiper evt -> 
                            FullScreen(JQuery(swiper.Container).Get 0)
                        )
                    )

                let galleryTop = new Swiper(".gallery-top", swipeParams)

                let thumbsParams = 
                    SwipeParameters(
                        SpaceBetween = 10,
                        SlidesPerView = Union1Of2 4,
                        FreeMode = true,
                        WatchSlidesVisibility = true,
                        WatchSlidesProgress = true,
                        Control = Union1Of2 galleryTop,
                        ControlBy = ControlBy.Slide,
                        OnTap = (fun swiper evt -> 
                            galleryTop.SlideTo swiper.ClickedIndex
                        )
                    )
                let galleryThumbs = new Swiper(".gallery-thumbs", thumbsParams)

                // synch thumbnails slider based on main slider position
                let fn = 
                    Function.Of(fun (swiper:Swiper) -> 
                        galleryThumbs.SlideTo swiper.ActiveIndex)

                galleryTop.On("onTouchEnd", fn) |> ignore
                JQuery("swiper-button-white")
                    .On("click",fun elem evt -> evt.StopPropagation())
                    |> ignore
            )
        
        div [ afterRenderAttr ] 
            [
                template().Doc()
            ]


