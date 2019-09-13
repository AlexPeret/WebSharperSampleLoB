namespace FrontEnd.Pages.Widgets

open System
open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Server
open WebSharper.Sitelets

[<JavaScript>]
module DateTimeJSPage =

    open FrontEnd.Helpers
    open FrontEnd.Webparts
    open FrontEnd.Config.Route

    module Services = FrontEnd.Server

    let private FormatLong (date:DateTime) =
        sprintf "%s - %s"
                (date.ToLongDateString()) (date.ToLongTimeString())

    let private FormatShort (date:DateTime) =
        sprintf "%s - %s"
                (date.ToShortDateString()) (date.ToShortTimeString())

    let private Row fn label value =
        tr []
           [ td [] [ text label ]
             td [] [ text (fn value) ]
           ]


    let Main go =
        let rowL = Row FormatLong
        let rowS = Row FormatShort

        let rows =
            async {
                let! dtServer = Services.CreateDate()
                let dtClient = DateTime.Now
                let! dtGetBack = Services.ReturnClientDate dtClient
                let! dtGetBackAsLocal = Services.ReturnClientDateAsLocal dtClient

                let dateTest:Services.DateTest = {
                    DateFromServer = dtServer
                    DateFromClient = dtClient
                    DateGetBack = dtGetBack
                }

                let! dtStructGetBack = Services.ReturnClientDateStruct dateTest
                let! dtStructGetBackAsLocal = Services.ReturnClientDateStructAsLocal dateTest

                let testSuite =
                    [
                      "Client side date",dtClient
                      "Server side date",dtServer
                      "Client side date sent and get back from server",dtGetBack
                      "Client side date sent to server and get back from server - converted to local",dtGetBackAsLocal
                      "Client side struct sent and get back from server - date from client",dtStructGetBack.DateFromClient
                      "Client side struct sent and get back from server - date from server",dtStructGetBack.DateFromServer
                      "Client side struct sent and get back from server - getback date",dtStructGetBack.DateGetBack
                      "Client side struct sent and get back from server - converted to local - date from client",dtStructGetBackAsLocal.DateFromClient
                      "Client side struct sent and get back from server - converted to local - date from server",dtStructGetBackAsLocal.DateFromServer
                      "Client side struct sent and get back from server - converted to local - getback date",dtStructGetBackAsLocal.DateGetBack
                    ]

                let longFormatTests =
                    testSuite
                    |> List.map (fun (label,value) -> rowL label value)
                let shortFormatTests =
                    testSuite
                    |> List.map (fun (label,value) -> rowS label value)

                return
                    longFormatTests@shortFormatTests
                    |> Doc.Concat
            }
            |> Doc.Async
        
        table 
            [ attr.``class`` "table table-bordered table-hover" ]
            [ thead [] [ tr [] [ th [] [ text "Test" ]; th [] [ text "DateTime" ] ] ]
              tbody [] [ rows ] ]
            
