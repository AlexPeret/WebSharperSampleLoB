namespace FrontEnd.Data

module DTO =
   open WebSharper.Html.Server.Attr

   type AttachmentModel  = {
        Id: int64
        FileName: string
        Mime: string
        CreateDate: string
    }

    type FileDownloadDTO = {
        Name: string
        Data: byte array
        Mime: string
    }

    type NotificationModel = {
        Icon : string
        Text : string
        Hours : string
        Link : string
    }
 

    type FilterOptions = {
        Location : (string*string)
        MainOptions : (int*string) list
        MainOption : int*string
        MinPrice : float option
        MaxPrice: float option
        Number : int option
    }

    type PropertyDTO = {
        Id: int64
        Location: string*string
        Title: string
        Subtitle: string
        Description: string
        Price: float
        Images: string list
    }

    type AccountModel = {
        Id: int64
        Location: string*string
        Email: string
        Genders: (int*string) list
        Gender: (int*string)
        Birthday: System.DateTime option
    }