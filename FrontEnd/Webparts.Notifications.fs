namespace FrontEnd.Webparts

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.JavaScript
open FrontEnd.Data.DTO

[<JavaScript>]
module Notification =
    type private template = Templating.Template<"templates/Webpart.Notification.html">

    let Run () = 
        let rvNotifications = 
            Var.Create []

        async {
            let! notifications = 
                FrontEnd.Server.ListOfNotifications ()
        
            Var.Set rvNotifications notifications 
        }
        |> Async.Start
        
        let items = 
            rvNotifications.View
            |> View.Map (fun notifications ->
                notifications
                |> List.map (fun notification ->
                     template.Notification()
                        .LinkIconAttr(attr.``class`` notification.Icon)
                        .LinkAttr(attr.href notification.Link )
                        .Text(notification.Text)
                        .Time(notification.Hours)
                        .Doc()
                )
                |> Doc.Concat
            )
            |> Doc.EmbedView
        
        template()
            .Notifications(items)
            .Doc()
