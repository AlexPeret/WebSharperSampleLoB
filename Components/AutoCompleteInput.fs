namespace Components.Forms

open System
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.Html.Client
open WebSharper.JQuery
open WebSharper.JQueryUI
open WebSharper

[<JavaScript>]
module AutoCompleteInput =

    type AutoCompleteItem<'a> = {
        Key: 'a
        Value: string
    }

    type AutoCompleteInputConfig<'a,'b>() =
        member val Callback = Unchecked.defaultof<string -> Async<('b * string) list>> with get, set
        member val OnChangeCallback = Unchecked.defaultof<AutoCompleteItem<'a> -> unit> with get, set
        member val CssClass = "form-control" with get, set
        member val DomID = "d" + System.Guid.NewGuid().ToString() with get, set
        member val Placeholder = "" with get, set
        member val MinLength = 2 with get, set


    let AutoCompleteInput<'a,'b> (rvItem: Var<AutoCompleteItem<'a>>) (config:AutoCompleteInputConfig<'a,'b>)  =
            
        let EmbedPagelet (pagelet : WebSharper.Html.Client.Element) render =
                pagelet.Dom
                |> View.Const
                |> View.Map (fun el ->
                    render ()
                    Doc.Static el)
                |> Doc.EmbedView 
                
        let AutoCompleter () =
            let el = Html.Client.Tags.Input []

            el.AddClass(config.CssClass)
            el.SetProperty("placeholder", config.Placeholder)
            el.SetProperty("id", config.DomID)

            let conf = new AutocompleteConfiguration()
            let completef (r : AutocompleteRequest, f : array<AutocompleteItem> -> unit) =
                //Note: o Value goes to input field, while label is used by the temporary listing
                async {
                    let! suggestions = config.Callback r.Term
                    let x =
                        suggestions
                        |> List.map (fun (k,v) -> { Label = v; Value = string (upcast k : obj) })
                        |> List.toArray
                    f x
                }
                |> Async.Start
            conf.Source <- Callback completef

            let completer = Autocomplete.New(el, conf)
            completer.OnSelect <| fun ev data ->
                let item:AutocompleteItem = Json.Decode data
                if rvItem.Value.Value <> item.Value then
                    let valueAsObj = upcast item.Value : obj
                    let value = unbox<'a> valueAsObj
                    Var.Set rvItem { Key = value; Value = item.Label }
                    //let value:'b = System.Convert.ChangeType(item.Value,typeof<'b>)
                    //Var.Set rvItem { Key = value; Value = item.Label }
                ev.PreventDefault()

            completer.OnFocus <| fun ev data ->
                // Workaround for WebSharper wrapper lack of support 
                // to AutocompleteItem on OnFocus signature;
                let asObj = upcast data : obj
                let asCompleteItem = downcast asObj : AutocompleteItem

                (* Replaces the key value by the label one. The original
                   behavior displays the key on the input field, as per
                   documentation. Cancelling the event stops this behavior. 
                   http://api.jqueryui.com/autocomplete/#event-focus
                   *)
                el.Value <- asCompleteItem.Label
                ev.PreventDefault()

            completer.OnChange <| fun ev data ->
                //let asType = upcast rvItem.Value 
                let keyAsObj = upcast rvItem.Value.Key : obj
                let key = unbox<'a> keyAsObj
                let item = { rvItem.Value with Key = key }
                config.OnChangeCallback item
                //config.OnChangeCallback rvItem.Value

            conf.MinLength <- config.MinLength
                        
            let doc = EmbedPagelet el completer.Render
                
            rvItem.View
            |> View.Map (fun newValue ->
                let adjustedNewValue = 
                    match newValue.Value with
                    | null -> ""
                    | v -> v
                el.Value <- adjustedNewValue
                doc)
            |> Doc.EmbedView
                
            
        AutoCompleter ()
