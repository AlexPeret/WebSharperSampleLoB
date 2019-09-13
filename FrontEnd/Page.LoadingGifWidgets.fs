namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open FrontEnd.Helpers
open WebSharper.JavaScript

[<JavaScript>]
module LoadingGifPage =
    open Components.Display.LoadingGif

    let Main go =
        let rShowGif = Var.Create false
        let rShowGifFullScreen = Var.Create false

        let cnt = 
            [
                LoadingGif rShowGif
                LoadingGifFull rShowGifFullScreen
            ]

        async {
            Var.Set rShowGifFullScreen true
            let! time = JS.Sleep 2000
            Var.Set rShowGifFullScreen false

            Var.Set rShowGif true
            let! time = JS.Sleep 2000
            Var.Set rShowGif false
        }
        |> Async.Start

        cnt
        |> Doc.Concat

