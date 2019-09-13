namespace FrontEnd.Config

open System
open WebSharper
open WebSharper.Resources

module Route =
    open WebSharper.Sitelets
    open WebSharper.UI

    [<JavaScript>]
    type EndPoint =
        | [<EndPoint "/">] Home
        | [<EndPoint "/widgets">] Widgets
        | [<EndPoint "/datepicker">] Datepicker
        | [<EndPoint "/autocomplete">] AutoComplete
        | [<EndPoint "/selectbox">] SelectBox
        | [<EndPoint "/breadcrumb">] Breadcrumb
        | [<EndPoint "/tab">] Tab
        | [<EndPoint "/googlemaps">] GoogleMaps
        | [<EndPoint "/slideshow">] SlideShow
        | [<EndPoint "/alert">] Alert
        | [<EndPoint "/progressbar">] ProgressBar
        | [<EndPoint "/attachments">] Attachments
        | [<EndPoint "/datetime-js">] DateTimeJS
        | [<EndPoint "POST /upload-attachment">] UploadAttachment
        | [<EndPoint "GET /download-attachment">] DownloadAttachment of attachId:int64
        | [<EndPoint "/help">] Help
        | [<EndPoint "/login">] Login
        | [<EndPoint "/restricted">] Restricted
        | [<EndPoint "/access-denied">] AccessDenied
        | [<EndPoint "/loading-gif">] LoadingGif
        | [<EndPoint "/sample-pages">] SamplePages
        // /listing-with-filters/location&main-option=?min-price=?max-price=?number=
        | [<EndPoint "/listing-with-filters">] 
          [<Query("mainOption", "minPrice", "maxPrice", "number")>]
          ListingWithFilters of location:string * mainOption:string option * minPrice:string option * maxPrice:string option * number:int option
        | [<EndPoint "/listing-with-actions">] ListingWithActions
        | [<EndPoint "/closed-form">] ClosedForm of eId:int64
        | [<EndPoint "/open-form">] OpenForm of eId:int64
        

    (* Router is used by both client and server side *)
    [<JavaScript>]
    let SiteRouter : Router<EndPoint> =
        let parseFiltersQuerystring (mapping:Map<string,string>) =
            let getValue (key:string) =
                mapping.TryFind key

            let vMainOption = getValue "main-option"
            let vMinPrice = getValue "min-price"
            let vMaxPrice = getValue "max-price"
            let vNumber = getValue "number" |> Option.map int
            vMainOption,vMinPrice,vMaxPrice,vNumber

        let link endPoint =
            let applyQuerystrings endpoint' paths =
                let mapping =
                    match endpoint' with
                    | ListingWithFilters (location,mainOption,minPrice,maxPrice,number) -> 
                        [
                            "main-option",mainOption
                            "min-price",minPrice
                            "max-price",maxPrice
                            "number",(Option.map string number)
                        ]
                        |> List.choose (fun (key,value) -> 
                            match value with
                            | None -> None
                            | Some value' -> Some (key,value')
                        )
                        |> Map.ofList
                    | _ -> Map.empty
                paths,mapping

            match endPoint with
            | Home -> [ ]
            | Widgets -> [ "widgets" ]
            | Datepicker -> [ "datepicker" ]
            | AutoComplete -> [ "autocomplete" ]
            | SelectBox -> [ "selectbox" ]
            | Breadcrumb -> [ "breadcrumb" ]
            | Tab -> [ "tab" ]
            | GoogleMaps -> [ "googlemaps" ]
            | SlideShow -> [ "slideshow" ]
            | Alert -> [ "alert" ]
            | ProgressBar -> [ "progressbar" ]
            | Attachments -> [ "attachments" ]
            | DateTimeJS -> [ "datetime-js" ]
            | UploadAttachment -> [ "upload-attachment" ]
            | DownloadAttachment attachId -> [ "download-attachment"; string attachId ]
            | Help -> [ "help" ]
            | Login -> [ "login" ]
            | Restricted -> [ "restricted" ]
            | AccessDenied -> [ "access-denied" ]
            | LoadingGif -> [ "loading-gif" ]
            | SamplePages -> [ "sample-pages" ]
            | ListingWithFilters (location,_,_,_,_) -> [ "listing-with-filters"; location ]
            | ListingWithActions -> [ "listing-with-actions" ]
            | ClosedForm eId -> [ "closed-form"; string eId ]
            | OpenForm eId -> [ "open-form"; string eId ]
            |> applyQuerystrings endPoint

        let route (path,mapping) =
            match path with
            | [] -> Some Home
            | [ "widgets" ] -> Some Widgets
            | [ "datepicker" ] -> Some Datepicker
            | [ "autocomplete" ] -> Some AutoComplete
            | [ "selectbox" ] -> Some SelectBox
            | [ "breadcrumb" ] -> Some Breadcrumb
            | [ "tab" ] -> Some Tab
            | [ "googlemaps" ] -> Some GoogleMaps
            | [ "slideshow" ] -> Some SlideShow
            | [ "alert" ] -> Some Alert
            | [ "progressbar" ] -> Some ProgressBar
            | [ "attachments" ] -> Some Attachments
            | [ "datetime-js" ] -> Some DateTimeJS 
            | [ "upload-attachment" ] -> Some <| UploadAttachment
            | [ "download-attachment"; attachId ] -> Some <| DownloadAttachment (int64 attachId)
            | [ "help" ] -> Some Help
            | [ "login" ] -> Some Login
            | [ "restricted" ] -> Some Restricted
            | [ "access-denied" ] -> Some AccessDenied
            | [ "loading-gif" ] -> Some LoadingGif
            | [ "sample-pages" ] -> Some SamplePages
            | [ "listing-with-filters"; location ] -> 
                let vMainOption,vMinPrice,vMaxPrice,vNumber = 
                    parseFiltersQuerystring mapping 
                Some <| ListingWithFilters (location,vMainOption,vMinPrice,vMaxPrice,vNumber)
            
            | [ "listing-with-actions" ] -> Some ListingWithActions
            | [ "closed-form"; eId ] -> Some <| ClosedForm (int64 eId)
            | [ "open-form"; eId ] -> Some <| OpenForm (int64 eId)
            | _ -> None
        Router.CreateWithQuery link route

    (* This sample illustrate how to setup a router for both server only
       and client/server applications. The RouteType is used by the 
       InstallRouter function to setup the router accordingly. 

       Notice that you must expand the ClientServer option as you add
       more Endpoints to the application. *)
    [<JavaScript>]
    type RouteType = ServerOnly | ClientServer

    (* This function returns a reactive variable used for client side
       routing. For Sitelets (server) only sample, pass ServerOnly option
       as parameter to turn off the client side router behavior. *)
    [<JavaScript>]
    let InstallRouter rType =
        match rType with
        | ServerOnly ->
            let turnOffClientRouting _ = None
            let router = 
                SiteRouter 
                |> Router.Slice
                    turnOffClientRouting // turns off client routing when it is a Sitelet only application
                    (fun endpoint -> endpoint)
                |> Router.Install Home
            router
        | ClientServer ->
            let router = 
                SiteRouter 
                |> Router.Slice
                    (fun endpoint ->
                        match endpoint with
                        | Home
                        | Widgets
                        | SamplePages
                        | UploadAttachment
                        | DownloadAttachment _
                        | Help 
                        | Login
                        | Restricted
                        | AccessDenied -> None
                        | _ -> Some endpoint
                    )
                    id
                |> Router.Install Home
            router


module UIResources =

    module Bootstrap =
        type Css() =
            inherit BaseResource("/vendor/bootstrap/css/bootstrap.min.css")

        [<Require(typeof<JQuery.Resources.JQuery>)>]
        type Js() =
            inherit BaseResource("/vendor/bootstrap/js/bootstrap.min.js")

    module FontAwesome =
        type Css() =
            inherit BaseResource("/vendor/font-awesome/css/font-awesome.min.css")

    module MetisMenu =
        type Js() =
            inherit BaseResource("/vendor/metisMenu/metisMenu.min.js")

    module FrontEndApp =
        type Css() =
            inherit BaseResource("/dist/css/common.css")

        type Js() =
            inherit BaseResource("/dist/js/common.js")

    module JQueryUIDatePickerPtBr =
        [<Require(typeof<JQuery.Resources.JQuery>)>]
        type Js() =
            inherit BaseResource("/vendor/jqueryui/jquery.ui.datepicker-pt-BR.js")

    module SlideShowSample =
        type Css() =
            inherit BaseResource("/dist/css/slideshowsample.css")

    module ListingWithFiltersSample =
        type Css() =
            inherit BaseResource("/dist/css/listing-with-filters-sample.css")

    [<assembly:Require(typeof<Bootstrap.Css>);
      assembly:Require(typeof<Bootstrap.Js>);
      assembly:Require(typeof<FontAwesome.Css>);
      assembly:Require(typeof<MetisMenu.Js>);
      assembly:Require(typeof<FrontEndApp.Css>);
      assembly:Require(typeof<FrontEndApp.Js>);
     >]
    do()


