namespace FrontEnd.Pages.Widgets

open WebSharper
open WebSharper.UI
open WebSharper.Sitelets

[<JavaScript>]
module AttachmentsPage =

    open FrontEnd.Helpers
    open FrontEnd.Webparts
    open FrontEnd.Config.Route

    module Services = FrontEnd.Server
    
    let Main go =
        let rvStatusMsg = Var.Create None
        let loadDataCallback () =
            Services.Attachments()

        let uploadPathFn () = 
            Link UploadAttachment

        let downloadPathFn attachmentId = 
            Link (DownloadAttachment attachmentId)

        Attachments.Run 
            go rvStatusMsg loadDataCallback uploadPathFn downloadPathFn
