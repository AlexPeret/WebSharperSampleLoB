namespace FrontEnd

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

open Config.Route

module Templating =
    open FrontEnd.Webparts

    type MainTemplate = Templating.Template<"Main.html">

    [<JavaScript>]
    let pendingCallback () = ()

    [<JavaScript>]
    let logoutCallback () =
        async {
            do! Auth.IdentitySignout ()
            Helpers.JS.Redirect <| Helpers.Link Home
        }
        |> Async.Start


    let Main (ctx:Context<EndPoint>) (action:EndPoint) (title: string) (body: Doc list) =
        let myAccountMenu =
            WebSharper.UI.Html.client <@ UserProfileMenu.Run pendingCallback pendingCallback logoutCallback @>
        let notificationMenu = 
            WebSharper.UI.Html.client <@ Notification.Run () @>
        Content.Page(
            MainTemplate()
                .Title(title)
                .AlertMenuRecentMessages(notificationMenu)
                .MyAccountMenu(myAccountMenu)
                .LeftMenu(LeftMenu.Server.Run ctx)
                .Body(body)
                .Doc()
        )

module Site =
    open WebSharper.UI.Html
    open FrontEnd.Helpers

    let private PageTitle endpoint =
        match endpoint with
        | Home -> "Home"
        | Widgets -> "Widgets and Samples"
        | Datepicker -> "Datepicker Widget"
        | AutoComplete -> "Autocomplete Widget"
        | SelectBox -> "Selectbox Widget"
        | Breadcrumb -> "Breadcrumb Widget"
        | Tab -> "Tab Widget"
        | GoogleMaps -> "GoogleMaps Sample"
        | SlideShow -> "SlideShow Sample"
        | Alert -> "Alert Widget"
        | ProgressBar -> "ProgressBar Widget"
        | Attachments -> "Attachments Sample"
        | DateTimeJS -> "DateTime JS Issue"
        | LoadingGif -> "LoadingGif Widget"
        | UploadAttachment -> "Upload Attachment Sample"
        | DownloadAttachment _ -> "Download Attachment Sample"
        | Login -> "Login Page"
        | Restricted -> "Resctricted Page"
        | AccessDenied -> "Access Denied Page"
        | Help -> "Documentation"
        | SamplePages -> "Samples Page"
        | ListingWithFilters _ -> "Listing with Filters Sample"
        | ListingWithActions _ -> "Listing with Actions Sample"
        | ClosedForm _ -> "Closed Form Sample"
        | OpenForm _ -> "Open Form Sample"
    
    let private UploadHandler ctx =
        let saveCallback n m fs = Error [ "not implemented" ]
        Server.UploadContent ctx saveCallback
    
    let private DownloadHandler ctx attachId =
        async {
            let! fileO = 
                Server.FindAttachment (fun attachment -> attachment.Id = attachId)
            let! content = 
                match fileO with
                | Some file -> 
                    Server.DownloadContent file
                | None -> 
                    Content.ServerError
            return content
        }


    (* Uncomment to test the server side only (Sitelet) version. *)
    //[<Website>]
    //let Main =
    //    Application.MultiPage (fun ctx endpoint ->
    //        match endpoint with
    //            | UploadAttachment -> UploadHandler ctx
    //            | DownloadAttachment attachId -> DownloadHandler ctx attachId
    //            | _ ->
    //                let page = 
    //                    match endpoint with
    //                    | EndPoint.Home -> 
    //                        client <@ Client.Main() @>
    //                    | EndPoint.Widgets -> 
    //                        client <@ Pages.Widgets.AllWidgetsPage.Main() @>
    //                    | EndPoint.Datepicker -> 
    //                        client <@ Pages.Widgets.DatePickerPage.Main <| GoDisabled() @>
    //                    | EndPoint.AutoComplete -> 
    //                        client <@ Pages.Widgets.AutoCompletePage.Main <| GoDisabled() @>
    //                    | EndPoint.SelectBox -> 
    //                        client <@ Pages.Widgets.SelectBoxPage.Main <| GoDisabled() @>
    //                    | EndPoint.Breadcrumb -> 
    //                        client <@ Pages.Widgets.BreadcrumbPage.Main <| GoDisabled() @>
    //                    | EndPoint.Tab -> 
    //                        client <@ Pages.Widgets.TabPage.Main <| GoDisabled() @>
    //                    | EndPoint.GoogleMaps -> 
    //                        client <@ Pages.Widgets.GoogleMapsPage.Main <| GoDisabled() @>
    //                    | EndPoint.SlideShow -> 
    //                        client <@ Pages.Widgets.SlideShowPage.Main <| GoDisabled() @>
    //                    | EndPoint.Alert -> 
    //                        client <@ Pages.Widgets.AlertPage.Main <| GoDisabled() @>
    //                    | EndPoint.ProgressBar -> 
    //                        client <@ Pages.Widgets.ProgressBarPage.Main <| GoDisabled() @>
    //                    | EndPoint.Attachments -> 
    //                        client <@ Pages.Widgets.AttachmentsPage.Main <| GoDisabled() @>
    //                    | EndPoint.DateTimeJS -> 
    //                        client <@ Pages.Widgets.DateTimeJSPage.Main <| GoDisabled() @>
    //                    | EndPoint.LoadingGif -> 
    //                        client <@ Pages.Widgets.LoadingGifPage.Main <| GoDisabled() @>
    //                    | UploadAttachment -> failwith "unexpected handler UploadAttachment"
    //                    | DownloadAttachment _ -> failwith "unexpected handler for DownloadAttachment"
    //                    | EndPoint.Help -> 
    //                        client <@ Pages.Help.Main() @>
    //                    | EndPoint.Restricted -> 
    //                        client <@ Pages.RestrictedPage.Main <| GoDisabled() @>
    //                    | AccessDenied ->
    //                        client <@ Pages.RestrictedPage.NoAccess <| GoDisabled() @>
    //                    | EndPoint.SamplePages -> 
    //                        client <@ Pages.Samples.SamplesPage.Main() @>
    //                    | EndPoint.ListingWithFilters (location,mainOption,minPrice,maxPrice,number) -> 
    //                        let parameters:Pages.Samples.ListingWithFiltersPage.Parameters = {
    //                            Location = location
    //                            MainOption = mainOption
    //                            MinPrice = minPrice
    //                            MaxPrice = maxPrice
    //                            Number = number
    //                        }
    //                        client <@ Pages.Samples.ListingWithFiltersPage.Main (GoDisabled()) parameters @>
    //                    | ClosedForm eId ->
    //                        client <@ Pages.Samples.ClosedFormPage.Main go eId @>
    //                    | OpenForm eId ->
    //                        client <@ Pages.Samples.OpenFormPage.Main go eId @>
    //                let title = PageTitle endpoint
    //                Templating.Main ctx endpoint title [ page ]
    //    )


    open WebSharper.UI.Client
    open WebSharper.Owin

    [<JavaScript>]
    let RouteClientPage () =
        let router = InstallRouter ClientServer
        let go = Go router

        let doc =
            router.View
            |> View.Map (fun endpoint -> 
                match endpoint with
                | Home -> [ Client.Main() ]
                | Widgets -> [ Pages.Widgets.AllWidgetsPage.Main() ]
                | Datepicker -> [ Pages.Widgets.DatePickerPage.Main go ]
                | AutoComplete -> [ Pages.Widgets.AutoCompletePage.Main go ]
                | SelectBox -> [ Pages.Widgets.SelectBoxPage.Main go ]
                | Breadcrumb -> [ Pages.Widgets.BreadcrumbPage.Main go ]
                | Tab -> [ Pages.Widgets.TabPage.Main go ]
                | GoogleMaps -> [ Pages.Widgets.GoogleMapsPage.Main go ]
                | SlideShow -> [ Pages.Widgets.SlideShowPage.Main go ]
                | Alert -> [ Pages.Widgets.AlertPage.Main go ]
                | ProgressBar -> [ Pages.Widgets.ProgressBarPage.Main go ]
                | Attachments -> [ Pages.Widgets.AttachmentsPage.Main go ]
                | DateTimeJS -> [ Pages.Widgets.DateTimeJSPage.Main go ]
                | LoadingGif -> [ Pages.Widgets.LoadingGifPage.Main go ]
                | UploadAttachment -> []
                | DownloadAttachment _ -> []
                | Help ->  [ Pages.Help.Main() ]
                | Login -> [ Pages.LoginPage.FormAuth (Link Restricted) ]
                | Restricted -> [ Pages.RestrictedPage.Main go ]
                | AccessDenied ->  [ Pages.RestrictedPage.NoAccess go ]
                | SamplePages -> [ Pages.Samples.SamplesPage.Main() ]
                | ListingWithFilters (location,mainOption,minPrice,maxPrice,number) -> 
                    let parameters:Pages.Samples.ListingWithFiltersPage.Parameters = {
                        Location = location
                        MainOption = mainOption
                        MinPrice = minPrice
                        MaxPrice = maxPrice
                        Number = number
                    }
                    [ Pages.Samples.ListingWithFiltersPage.Main go parameters ]
                | ListingWithActions -> [ Pages.Samples.ListingWithActionsPage.Main go ]
                | ClosedForm eId ->
                    [ Pages.Samples.ClosedFormPage.Main go eId ]
                | OpenForm eId ->
                    [ Pages.Samples.OpenFormPage.Main go eId ]
                |> Doc.Concat
            )
            |> Doc.EmbedView

        doc

    let LoadClientPage ctx endpoint =
        let body = client <@ RouteClientPage() @>

        Templating.Main ctx endpoint "WebSharper Sample LoB" [ body ]

    [<Website>]
    let Main =
        Sitelet.New 
            SiteRouter 
            (fun ctx endpoint ->

                let loggedUser = 
                    async {
                        return! Auth.IsLogged ctx
                    } |> Async.RunSynchronously

                let title = PageTitle endpoint
                let secureMainTemplate body = 
                    match loggedUser with
                    | None -> 
                        // redirect if not logged
                        match endpoint with
                        | Restricted -> Content.RedirectTemporary AccessDenied
                        | _ -> Templating.Main ctx endpoint title body
                    | Some (u) -> 
                        // redirect if logged
                        match endpoint with
                        | Login -> Content.RedirectTemporary Restricted
                        | _ -> Templating.Main ctx endpoint title body
                    

                match endpoint with
                | Home -> 
                    secureMainTemplate [ client <@ Client.Main() @> ]
                | Widgets -> 
                    secureMainTemplate [ client <@ Pages.Widgets.AllWidgetsPage.Main() @> ]
                | Help ->  
                    secureMainTemplate [ client <@ Pages.Help.Main() @> ]
                | Login ->  
                    secureMainTemplate [ client <@ Pages.LoginPage.FormAuth (Link Restricted) @> ]
                | Restricted ->
                    secureMainTemplate [ client <@ Pages.RestrictedPage.Main <| GoDisabled() @> ]
                | AccessDenied ->
                    secureMainTemplate [ client <@ Pages.RestrictedPage.NoAccess <| GoDisabled() @> ]
                | SamplePages -> 
                    secureMainTemplate[ client <@ Pages.Samples.SamplesPage.Main() @> ]
                | UploadAttachment -> UploadHandler ctx
                | DownloadAttachment attachId -> DownloadHandler ctx attachId
                | Datepicker 
                | AutoComplete
                | SelectBox
                | Breadcrumb
                | Tab
                | GoogleMaps 
                | SlideShow 
                | ProgressBar
                | Attachments
                | DateTimeJS
                | LoadingGif
                | Alert
                | ListingWithFilters _ 
                | ListingWithActions 
                | ClosedForm _ 
                | OpenForm _ -> LoadClientPage ctx endpoint
            )

