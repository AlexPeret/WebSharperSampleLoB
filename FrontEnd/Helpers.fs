namespace FrontEnd

open System
open System.IO
open WebSharper
open WebSharper.Sitelets
open WebSharper.Sitelets.Http
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.JavaScript

module Helpers =
    open FrontEnd.Config.Route

    let ReadLines (filePath:string) =
        let reader = 
            new StreamReader(filePath) 
            |> Seq.unfold (fun sr -> 
                match sr.ReadLine() with
                | null -> sr.Dispose(); None 
                | str -> Some(str, sr))
        reader |> Seq.toList

    let readBuffer (data:Stream) =
        fun (out:Stream) ->
            let buffer = Array.zeroCreate (16 * 1024)
            let rec loop () =
                let read = data.Read(buffer, 0, buffer.Length)
                if read > 0 then out.Write(buffer, 0, read); loop ()
            loop ()

    let TryAsDateTime v =
        let res = ref System.DateTime.MinValue
        if System.DateTime.TryParse (v, res) then
            Some !res
        else
            None

    [<JavaScript>]
    let StringToOption s =
        match s with
        | null
        | "" -> None
        | _ -> Some s

    [<JavaScript>]
    let AsOption defaultValue f d =
        match d with
        | None -> defaultValue
        | Some d' -> f d'

    [<JavaScript>]
    let Link act =
        Router.Link SiteRouter act

    [<JavaScript>]
    let LinkDoc act content =
        a [ attr.href (Link act) ] [ text content ]
    
    [<JavaScript>]
    let Go router = 
        Var.Set router

    (* The router will be install once at the first call to Go val *)
    [<JavaScript>]
    let GoDisabled () = 
        Go <| InstallRouter ServerOnly

    type AsyncResult<'Success,'Failure> = 
        Async<Result<'Success,'Failure>>

    let GetHeaderValue (ctx:Context<EndPoint>) key =
        ctx.Request.Headers 
        |> Seq.filter (fun h -> h.Name = key)
        |> Seq.map (fun h -> System.Web.HttpUtility.UrlDecode(h.Value))
        |> Seq.exactlyOne

    let ResultToContent res = 
        match res with
        | Error (erro:string list) -> 
            let errorStream = new MemoryStream()
            let sw = new StreamWriter(errorStream)

            erro 
            |> List.iter(fun e -> sw.Write(e))

            sw.Flush()
            errorStream.Position <- 0L

            Content.Custom(
                Status = Status.InternalServerError,
                Headers = [],
                WriteBody = (readBuffer errorStream)) 
        | Ok status ->
            Content.Text status

    [<JavaScript>]
    module JS =
    
        [<Inline "document.location = $location">]
        let Redirect (location: string) = X<unit>

        let Sleep miliseconds =
            Promise(fun (resolve, _) ->
                JS.SetTimeout (fun () -> resolve 42) miliseconds |> ignore)
            |> Promise.AsTask
            |> Async.AwaitTask

