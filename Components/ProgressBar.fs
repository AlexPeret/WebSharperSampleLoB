namespace Components.Display

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client

[<JavaScript>]
module ProgressBar =

    (* This component is created as a class type in order to allow having
       more than one component in the page at the same time. *)
    type ProgressBarT() =

        let rvDisplayProgressBar = Var.Create false
        let rvProgressBarWidth = Var.Create 0.

        member this.Restart () =
            Var.Set rvDisplayProgressBar true
            Var.Set rvProgressBarWidth 0.

        member this.Hide () =
            Var.Set rvDisplayProgressBar false
            Var.Set rvProgressBarWidth 0.
        
        member this.Step width =
            if not rvDisplayProgressBar.Value then
                Var.Set rvDisplayProgressBar true
            
            Var.Set rvProgressBarWidth width

        member this.ProgressBar () =
            let vDisplayProgressBar =
                rvDisplayProgressBar.View
                |> View.Map not
            let vHidden = View.Const "hidden"

            let vProgressBarWidth = 
                rvProgressBarWidth.View
                |> View.Map (fun w -> sprintf "width:%i%%" (int w))

            div 
              [ attr.classDynPred vHidden vDisplayProgressBar ] 
              [ div
                  [ attr.``class`` "progress" ] 
                  [ div 
                      [ attr.styleDyn vProgressBarWidth
                        attr.``class`` "progress-bar" ]
                      [ ]
                  ]
              ]
