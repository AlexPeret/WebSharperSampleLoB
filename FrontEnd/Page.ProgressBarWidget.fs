namespace FrontEnd.Pages.Widgets

open WebSharper
open FrontEnd.Helpers

[<JavaScript>]
module ProgressBarPage =

    open Components.Display.ProgressBar
    
    let Main go =
        let progressBar = ProgressBarT()
        let progressBarDoc = progressBar.ProgressBar()
        
        let steps = [0. .. 10. .. 100.]

        // The Sleep function won't work inside List.iter function
        //steps
        //|> List.iter(fun step ->
        //    async {
        //        let! time = Sleep 1000 
        //        // simulates slow processing
        //        progressBar.Step(step)
        //        if step = (steps |> List.last) then
        //            let! time = Sleep 1000 
        //            progressBar.Hide()
        //    }
        //    |> Async.Start
        //) 

        async {
            for step in steps do
                let! time = JS.Sleep 600 
                // simulates slow processing
                progressBar.Step(step)
                if step = (steps |> List.last) then
                    let! time = JS.Sleep 3000 
                    progressBar.Hide()
        }
        |> Async.Start

        progressBarDoc
