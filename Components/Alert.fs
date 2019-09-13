namespace Components.Display

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
module Alert = 
    (* Resulting HTML:
        <div class="alert alert-success alert-dismissable">
          <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
          Lorem ipsum dolor sit amet, consectetur adipisicing elit.
        </div>
        <button class="close" data-alert="alert-hidden">×</button>
    *)

    type AlertStyles = 
        | Success of string
        | Info of string
        | Warning of string
        | Danger of string list

    let private MapColorClass color = 
         match color with
         | Success _ -> "alert-success"
         | Info _ -> "alert-info"
         | Warning _ -> "alert-warning"
         | Danger _ -> "alert-danger"

    let private mapToUL msgs = 
        let msgItems = 
            msgs
            |> List.map (fun msg ->
                li [] [ text msg ]
            )

        ul
          []
          msgItems
        

    let Alert (colorMessage:Var<AlertStyles>) =
        let vCor = 
            colorMessage.View
            |> View.Map (fun color ->
                let colorCss = MapColorClass color
                let divClass = "alert " + colorCss + " alert-dismissable"
                divClass
            )
        let vMessage =
            colorMessage.View
            |> View.Map (fun color ->
                match color with
                | Success msg -> text msg
                | Info msg -> text msg
                | Warning msg -> text msg
                | Danger msgs -> mapToUL msgs
            )

        let hasMsg s = s <> ""
        let hasMsgs l = List.isEmpty >> not <| l

        let vDisplay =
            colorMessage.View
            |> View.Map (fun color ->
                match color with
                | Success msg -> hasMsg msg
                | Info msg -> hasMsg msg
                | Warning msg -> hasMsg msg
                | Danger msgs -> hasMsgs msgs
            )

        let alertBoxElem = 
            div
              [attr.classDyn vCor ]
              [ button
                  [ attr.``class`` "close" 
                    attr.``data-`` "dismiss" "alert"
                    Attr.Create "aria-hidden" "true"
                    attr.``type`` "button"
                  ]
                  [ Doc.Verbatim "&times;"
                  ] 
                vMessage |> Doc.EmbedView
              ]
        
        vDisplay
          |> View.Map(fun show ->
            if show then
                alertBoxElem
            else
                Doc.Empty
          )
          |> Doc.EmbedView

    let AlertResult (result:Var<Result<string,string list>>) =
        let var2 = 
            Var.Lens result
                     (fun res -> 
                        match res with
                        | Ok msg -> Success msg
                        | Error msgs -> Danger msgs
                     )
                     (fun res _ -> res) // ignore update
        Alert var2

