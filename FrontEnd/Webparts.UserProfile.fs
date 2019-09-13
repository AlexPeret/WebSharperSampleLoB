namespace FrontEnd.Webparts

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.JavaScript
open WebSharper.FileDropJs

[<JavaScript>]
module UserProfileMenu =

    type private template = Templating.Template<"templates/Webpart.UserProfileMenu.html">
    
    let Run userProfileCallback configCallback logOutCallback =
        template()
            .UserProfileClick(fun _ -> userProfileCallback())
            .ConfigClick(fun _ ->  configCallback())
            .LogOutClick(fun _ -> 
                logOutCallback()
                //async {
                //    do! Auth.Logout ()
                //    Helpers.JS.Redirect logoutRedirUrl
                //}
                //|> Async.Start
            )
            .Doc()
        
