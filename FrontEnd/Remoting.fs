namespace FrontEnd

open WebSharper
open WebSharper.Sitelets

module Server =

    open System
    open System.IO
    open System.Web
    open FrontEnd.Data.DTO
    open FrontEnd.Helpers
    open FrontEnd.Config.Route

    [<JavaScript>]
    type DateTest = {
        DateFromServer: DateTime
        DateFromClient: DateTime
        DateGetBack: DateTime
    }


    [<Rpc>]
    let DoSomething input =
        let R (s: string) = System.String(Array.rev(s.ToCharArray()))
        async {
            return R input
        }

    [<Rpc>]
    let CreateDate () =
        async {
            // The date is created using the current location
            let date = DateTime.Now
            System.Diagnostics.Trace.WriteLine("CreateDate:")
            System.Diagnostics.Trace.WriteLine(date)
            return date
        }

    [<Rpc>]
    let ReturnClientDate (date:DateTime) =
        async {
            // The date created at the client is passed in UTC format and is handled
            // correctly by Javascript (at the client side) when returned as is.
            System.Diagnostics.Trace.WriteLine("ReturnClientDate:")
            System.Diagnostics.Trace.WriteLine(date)
            return date
        }
    [<Rpc>]
    let ReturnClientDateAsLocal (date:DateTime) =
        async {
            // The date created at the client is passed in UTC format and is handled
            // correctly by Javascript (at the client side) when converted to local.
            System.Diagnostics.Trace.WriteLine("ReturnClientDateAsLocal:")
            System.Diagnostics.Trace.WriteLine(date)

            let localDate = date.ToLocalTime()
            System.Diagnostics.Trace.WriteLine(localDate)
            return localDate
        }

    [<Rpc>]
    let ReturnClientDateStruct (dateS:DateTest) =
        async {
            System.Diagnostics.Trace.WriteLine("ReturnClientDateStruct")
            System.Diagnostics.Trace.WriteLine(dateS)
            return dateS
        }

    [<Rpc>]
    let ReturnClientDateStructAsLocal (dateS:DateTest) =
        async {
            System.Diagnostics.Trace.WriteLine("ReturnClientDateStructAsLocal")
            let localDateS =
                { dateS with
                    DateFromServer = dateS.DateFromServer.ToLocalTime()
                    DateFromClient = dateS.DateFromClient.ToLocalTime()
                    DateGetBack = dateS.DateGetBack.ToLocalTime()
                }
            System.Diagnostics.Trace.WriteLine(localDateS)
            return localDateS
        }


    let HelpContentRaw () =
        let docsPath = 
            Path.Combine(HttpRuntime.AppDomainAppPath, "docs.txt")
        File.ReadAllBytes docsPath

    [<Rpc>]
    let HelpContent () =
        let docsPath = 
            Path.Combine(HttpRuntime.AppDomainAppPath, "docs.txt")

        async {
            let fileRows = 
                Helpers.ReadLines docsPath 
                |> Seq.toList

            return fileRows
        }

    [<Rpc>]
    let ListOfFruits searchTerm = 
        let fruits = 
            [ 1L,"apple"
              2L,"avocado"
              3L,"banana"
              4L,"blackberry"
              5L,"blueberry"
              6L,"cherry"
              7L,"pear"
              8L,"pineapple"
            ]
        async {
            return
                fruits
                |> List.filter(fun (key,value) -> 
                    value.StartsWith(searchTerm)
                )
        }

    [<Rpc>]
    let ListOfFruits2 searchTerm = 
        let fruits = 
            [
              3L,"banana"
              4L,"blackberry"
              5L,"blueberry"
              6L,"cherry"
            ]
        async {
            return
                fruits
                |> List.filter(fun (key,value) -> 
                    value.StartsWith(searchTerm)
                )
        }

    [<Rpc>]
    let ListOfOptions () =
        let options = 
            [
                2,"Option2"
                3,"Option3"
                4,"Option4"
            ]
        async {
              do! Async.Sleep 3000
              return
                   options
        }

    [<Rpc>]
    let ListOfNotifications () = 
        let notifications = 
            [
                {
                    Icon = "fa fa-comment fa-fw" 
                    Text = "Comment 1" 
                    Hours = "4 minutes ago" 
                    Link = Helpers.Link Home
                }
                {
                    Icon = "fa fa-comment fa-fw" 
                    Text = "Comment 2" 
                    Hours = "6 minutes ago" 
                    Link = Helpers.Link Widgets
                }
                {
                    Icon = "fa fa-comment fa-fw" 
                    Text = "Comment 3" 
                    Hours = "12 minutes ago"
                    Link = Helpers.Link SamplePages
                }
            ]
        async {
              do! Async.Sleep 3000
              return
                   notifications
        }

    let AllLocations () =
        [
            "ouro-preto-mg","Ouro Preto - MG"
            "mariana-mg","Mariana - MG"
        ]

    [<Rpc>]
    let DefaultFilterModel () =
        async {
            let! mainOptions = ListOfOptions()
            return 
                {
                    Location = AllLocations() |> List.head
                    MainOptions = mainOptions
                    MainOption = mainOptions |> List.head
                    MinPrice = None
                    MaxPrice = None
                    Number = None
                }
        }
    
    [<Rpc>]
    let FindLocations (name:string) =
        async {
            return
                AllLocations()
                |> List.filter (fun l ->
                    if System.String.IsNullOrWhiteSpace(name) then
                        true
                    else
                        let locationName = snd l
                        locationName.StartsWith locationName
                )
        }
    
    [<Rpc>]
    let GetLocation (location:string) =
        async {
            return
                AllLocations()
                |> List.filter (fun (key,_) -> key = location)
                |> List.tryHead
        }

    [<Rpc>]
    let GetDefaultLocation () =
        async {
            return
                AllLocations()
                |> List.head
        }

    [<Rpc>]
    let FindProperties filters =
        let images =
            [
                "/images/pexels-photo-106399.jpeg"
                "/images/pexels-photo-186077.jpeg"
            ]
        let locations =  AllLocations()

        let properties = 
            [ 1L..10L ]
            |> List.map (fun i ->
                {
                    Id = i
                    Location = locations.Item(int i % 2)
                    Title = sprintf "Title for item %u" i
                    Subtitle = sprintf "Subtitle for item %u" i
                    Description = 
                        sprintf "Short description for item %u. Short description for item %u. " i i
                    Price = (float i * 1000.)
                    Images = images
                }
            )
            |> List.filter(fun i ->
                let predMin = 
                    match filters.MinPrice with
                    | None -> true
                    | Some mp -> i.Price >= mp
                let predMax =
                    match filters.MaxPrice with
                    | None -> true
                    | Some mp -> i.Price <= mp
                let predLocation = i.Location = filters.Location
                predMin && predMax && predLocation
            )

        async {
            return properties
        }

    let private SampleAttachments = 
        [
            { Id = 1L; FileName = "file 1.txt"; Mime = "text/plain"; CreateDate = "21/01/2019" }
            { Id = 2L; FileName = "file 2.txt"; Mime = "text/plain"; CreateDate = "23/01/2019" }
        ]

    let FindAttachment predicate =
        async {
            return 
                SampleAttachments
                |> List.filter predicate
                |> List.map (fun attachment -> 
                    {
                        Name = attachment.FileName
                        Data = HelpContentRaw()
                        Mime = attachment.Mime
                    }
                )
                |> List.tryHead
        }
    
    [<Rpc>]
    let Attachments () : Async<AttachmentModel list> =
        async {
            return SampleAttachments
        }

    [<Rpc>]
    let RemoveAttachment (attachment:AttachmentModel): AsyncResult<string,string list> =
        async {
            return Error [ "not implemented" ]
        }

    let DownloadContent (file:FileDownloadDTO) =
        let dadosStream = new System.IO.MemoryStream(file.Data)
        Content.Custom(
            Status = Http.Status.Ok,
            Headers = 
                [ Http.Header.Custom "Content-Type" file.Mime
                  Http.Header.Custom "Content-Disposition" (sprintf "attachment; filename=\"%s\"" file.Name)
                ],
            WriteBody = (readBuffer dadosStream))


    let UploadContent (ctx:Context<EndPoint>) salvarCallback =
        let mime = GetHeaderValue ctx "X-File-Type"

        match Array.ofSeq ctx.Request.Files with
        | [| f |] ->
            use fileStream = new System.IO.MemoryStream()
            f.InputStream.Seek(0L, System.IO.SeekOrigin.Begin) |> ignore
            f.InputStream.CopyTo(fileStream)
            let res = salvarCallback f.FileName mime fileStream

            fileStream.Close()

            ResultToContent res
        | _ ->
            match ctx.Request.Body.Length with
            | 0L ->
                Content.Text "The file is required"
                |> Content.SetStatus (Http.Status.Custom 400 (Some "Bad Request"))
            | _ ->
                let fileName = 
                    ctx.Request.Headers 
                    |> Seq.filter (fun h -> h.Name = "X-File-Name")
                    |> Seq.map (fun h -> System.Web.HttpUtility.UrlDecode(h.Value))
                    |> Seq.exactlyOne

                use fileStream = new System.IO.MemoryStream()
                ctx.Request.Body.Seek(0L, System.IO.SeekOrigin.Begin) |> ignore
                ctx.Request.Body.CopyTo(fileStream)

                let res = salvarCallback fileName mime fileStream

                fileStream.Close()

                ResultToContent res

    let Genders () =
        [ 0,"Not provided"
          1,"Female"
          2,"Male"]

    [<Rpc>]
    let GetAccountInfo (eId:int64) =
        async {
            return {
                Id = eId
                Location = AllLocations() |> List.head
                Email = "user@home.com"
                Genders = Genders()
                Gender = Genders() |> List.item 2
                Birthday = Some <| DateTime(1980, 04, 23, 0, 0, 0)
            }
        }
    
    [<Rpc>]
    let SaveAccountInfo (model:AccountModel) =
        let isValid = 
            if model.Location = ("","") then
                Error [ "location is required" ]
            elif model.Email = "" then
                Error [ "email is required" ]
            elif model.Gender = (Genders() |> List.head) then
                Error [ "gender is required" ]
            elif model.Birthday.IsNone then
                Error [ "birthday is required" ]
            else
                Ok "account saved"
                
        async {
            do! Async.Sleep 2000
            return 
                isValid
        }