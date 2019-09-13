namespace FrontEnd.Webparts

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.JavaScript
open WebSharper.FileDropJs

[<JavaScript>]
module Attachments =

    open FrontEnd.Data.DTO
    open Components.Display
    module Service = FrontEnd.Server

    type private template = Templating.Template<"templates/Webpart.Attachments.html">

    let documentsModel = 
        ListModel.Create (fun (i:AttachmentModel) -> i.Id) []

    let UploadCallback callback =
        async {
            let! attachments = callback ()
            documentsModel.Set <| attachments
        } |> Async.Start

    let private removeCallback (rvStatusMsg:Var<Result<string,string list> option>) 
        (attachment:AttachmentModel) loadAttachmentsCallback = 
        async {
            let! response =
                Service.RemoveAttachment attachment

            match response with
            | Error err -> rvStatusMsg.Value <- Some response
            | Ok model -> 
                UploadCallback loadAttachmentsCallback
        } |> Async.Start

    
    let private LinkDownload fileName fileLink =
        a [ attr.href fileLink; attr.target "_blank" ] [ text fileName ]

    let Run go (rvStatusMsg:Var<Result<string,string list> option>) 
        loadAttachmentsCallback uploadPathCallback downloadPathCallback =

        let progressBar = ProgressBar.ProgressBarT()
        let progressBarComp = progressBar.ProgressBar()

        // As the template is attached to DOM after it is executed, we must
        // postpone the FileDrop call, otherwise, it won't find the uploadWidget
        // element when executed.
        let registerOnReadyFn (elem:Dom.Element) = 
            let dd = new FileDrop("uploadWidget")
            dd.Event("send", fun files ->
                files.Each(fun file ->
                    // resets the progress bar for each file
                    file.Event("xhrSend", fun xhr data opt ->
                        progressBar.Restart ()
                    )
                    file.Event("progress", fun current total xhr ev ->
                        let width = current / total * 100.
                        progressBar.Step width
                    )

                    file.Event("done", fun (xhr:XMLHttpRequest) ev ->
                        UploadCallback loadAttachmentsCallback
                        progressBar.Hide()
                    )
                    file.Event("error", fun evt (xhr:XMLHttpRequest) ->
                        Var.Set rvStatusMsg (Some <| Result.Error [ xhr.StatusText ])
                        progressBar.Hide()
                    )
                    file.SendTo(uploadPathCallback():string) |> ignore
                )
                |> ignore
            )
            |> ignore

        let cnt = 
            async {
                let! attachments = loadAttachmentsCallback ()

                documentsModel.Set attachments

                let rows =
                    ListModel.View documentsModel 
                    |> Doc.BindSeqCached (fun (doc:AttachmentModel) ->
                        let icone,nomeElem = 
                            let pathDownloadFile = 
                                downloadPathCallback doc.Id
                            let linkToFile =
                                LinkDownload doc.FileName pathDownloadFile
                            "fa-file-pdf-o",linkToFile

                        template
                            .FileRow()
                            .IconClassName(icone)
                            .Name(nomeElem)
                            .CreateDate(doc.CreateDate)
                            .Remove(fun _ -> 
                                removeCallback rvStatusMsg doc loadAttachmentsCallback)
                            .Doc())

                return 
                    template
                        .Main()
                        .ProgressBar(progressBarComp)
                        .FileList(rows)
                        .Doc()
            } |> Doc.Async

        let bodyPanel = 
            div [ on.afterRender registerOnReadyFn ] [ cnt ]

        bodyPanel


