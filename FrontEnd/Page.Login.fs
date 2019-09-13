namespace FrontEnd.Pages

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.JQuery

[<JavaScript>]
module LoginPage =

    open FrontEnd

    type LoginPageTemplate = Templating.Template<"templates/Page.FormAuth.html">

    let FormAuth redirUrl =
        let rvEmail = Var.Create ""
        let rvPassword = Var.Create ""
        let rvKeepLogged = Var.Create false
        let rvStatusMsg = Var.Create None

        let linearAnim = Anim.Simple Interpolation.Double (Easing.Custom id) 300.
        let cubicAnim = Anim.Simple Interpolation.Double Easing.CubicInOut 300.
        let swipeTransition =
            Trans.Create linearAnim
            |> Trans.Enter (fun x -> cubicAnim (x - 100.) x)
            |> Trans.Exit (fun x -> cubicAnim x (x + 100.))
        let rvLeftPos = Var.Create 0.
        let movElem = 
            div 
              [ Attr.Style "position" "relative"
                Attr.AnimatedStyle "left" swipeTransition rvLeftPos.View (fun pos -> string pos + "%")
              ]
              [ Doc.TextNode "fill up the form"
              ]

        let statusMsgBox = 
            rvStatusMsg.View
            |> View.Map (function
                | None -> movElem
                | Some "" -> Doc.Empty
                | Some t -> LoginPageTemplate.AlertBoxTemplate().Message(t).Doc()
            ) |> Doc.EmbedView

        LoginPageTemplate()
            .rvLogin(rvEmail)
            .rvPwd(rvPassword)
            .rvRemember(rvKeepLogged)
            .AlertBox(statusMsgBox)
            .GetLogged(fun _ ->
                JQuery.Of("form").One("submit", fun elem ev -> ev.PreventDefault()).Ignore
                async {
                    let! response = 
                        Auth.CheckCredentials rvEmail.Value rvPassword.Value rvKeepLogged.Value
                    match response with
                    | Ok c ->
                        rvEmail.Value <- ""
                        rvPassword.Value <- ""
                        rvStatusMsg.Value <- None
                        Helpers.JS.Redirect redirUrl
                    | Error errors -> 
                        let msg =
                            errors 
                            |> List.fold (fun i s -> i + s) ""
                        rvStatusMsg.Value <- Some msg
                } |> Async.Start
            )
            .Doc()

    let LogOut redirUrl =
        async {
            //do! Auth.Logout ()
            do! Auth.IdentitySignout ()
            Helpers.JS.Redirect redirUrl
            return Doc.Empty
        }
        |> Doc.Async


