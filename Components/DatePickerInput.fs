namespace Components.Forms

open System
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.Html.Client
open WebSharper.JQueryUI
open WebSharper


[<JavaScript>]
module DatePickerInput =

    type MaybeDate = 
        | ValidDate of DateTime option
        | InvalidDate of string

    let private DateOToS d =
        match d with
        | None -> ""
        | Some (d:DateTime) -> d.ToShortDateString()

    let private MaybeDateTryParse s =
        match s with
        | null | "" -> InvalidDate s
        // 00/00/0000 (Length = 10)
        | _ when s.Length = 10 -> 
            let dataA = s.Split([|'/'|])
            let dataSUs = 
                sprintf "%s/%s/%s" dataA.[1] dataA.[0] dataA.[2]
            // WARNING: this doesn't work correctly on MS Edge
            let isValid,date = System.DateTime.TryParse dataSUs
            match isValid with
            | false -> ValidDate None
            | true -> ValidDate <| Some date
        | _ -> InvalidDate s

    let CreateMaybeDate dateO =
        ValidDate dateO

    let private UpdateDate mDate dataO =
        match dataO with
        | None -> mDate
        | Some (d:DateTime) -> ValidDate dataO

    let private MaybeDateFormat mDate =
        match mDate with
        | ValidDate d -> DateOToS d
        | InvalidDate s -> s

    (* Creates a input field bound to a reactive variable for JQuery UI Datepicker
       widget.

       If you need internationalization support, do the following:
       - set the correct UI Culture. The line below is a Web.Config alternative:
          ...
          <system.web>
            <globalization culture="pt-BR" uiCulture="pt-BR"/>
            ...
        - add support to jQuery UI Datepicker i18n
        
        Usage:
        let config =
            DatepickerConfiguration(
                ShowOn = "focus",
                DateFormat = "dd/mm/yy",
                ChangeMonth = true,
                ChangeYear = true,
                AutoSize = true
            )

        let rvDate = Var.Create <| MaybeDate.Create None

        let pickDoc = DatePickerField rvDate config
    *)
    let DatePickerInput (rv:Var<MaybeDate>) config =

        let el' = Input []
        let el =
            el'.OnKeyUp(fun elem evt -> 
                // synch the value attribute with the value typed
                let currentValue = elem.Value
                let mDate = MaybeDateTryParse currentValue
                Var.Set rv mDate
            )

        let picker = Datepicker.New(el,config)

        picker.OnSelect (fun dt elem ->
            Var.Update rv (fun c -> UpdateDate c <| Some dt.Self)
        )
        // scenario: when user types in the date
        let rvDateS = 
            Var.Lens 
                rv
                (fun a -> MaybeDateFormat a)
                (fun a v -> MaybeDateTryParse v)

        let pickerDoc = 
            el.Dom
            |> View.Const
            |> View.Map (fun elem ->
                elem.SetAttribute("class", "form-control")
                let rvDv =
                    rvDateS.View
                    |> View.Map (fun d ->
                        el.Value <- d
                        Doc.Static elem
                    )
                    |> Doc.EmbedView

                picker.Render ()
                rvDv)
            |> Doc.EmbedView 

        let cnt = 
            rv.View
            |> View.Map (fun newValue ->
                match newValue with
                | InvalidDate _ -> pickerDoc
                | ValidDate d' ->
                    match d' with
                    | None -> pickerDoc
                    | Some d -> 
                        let currentDate = picker.GetDate()
                        if currentDate <> null && d <> currentDate.Self then
                            picker.SetDate d.JS
                        pickerDoc)
            |> Doc.EmbedView

        cnt

