namespace FrontEnd.Pages.Widgets

open System
open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.JQueryUI

[<JavaScript>]
module DatePickerPage =

    open Components.Forms.DatePickerInput
    open Components.Display

    type private template = Templating.Template<"templates/Module.DatepickerWidget.html">

    (* Reference: https://try.websharper.com/snippet/Lamk/000047
        Note: this lib cannot be called here, because WebSharper 
        will inject it before jqueryui lib. *)
    [<Require(typeof<FrontEnd.Config.UIResources.JQueryUIDatePickerPtBr.Js>)>]
    let private DatepickerTest () =
        let config =
            DatepickerConfiguration(
                //ShowOn = "focus",
                DateFormat = "dd/mm/yy",
                ChangeMonth = true,
                ChangeYear = true,
                AutoSize = true
            )

        let rvDate = Var.Create <| CreateMaybeDate None
        let pickDoc = 
            async {
                return DatePickerInput rvDate config
            } |> Doc.Async
        
        let newDate = (DateTime.Now.AddDays(float 2))
        Var.Set 
            rvDate
            (CreateMaybeDate (Some newDate))

        rvDate,pickDoc

    let Main go =
        let rStatusMsg = Var.Create (Ok "")
        let alertBox = Alert.AlertResult rStatusMsg

        let rvDate,datePickerDoc = DatepickerTest()
        let vDate =
            rvDate.View
            |> View.Map(fun mDate -> 
                match mDate with
                | InvalidDate s -> 
                    let msg = sprintf "invalid date: %s" s
                    rStatusMsg.Value <- Result.Error [ msg ]
                    msg
                | ValidDate d' -> 
                    match d' with
                    | None -> 
                        let msg = "no date selected"
                        rStatusMsg.Value <- Result.Error [ msg ]
                        msg
                    | Some d -> sprintf "valid date: %s" (d.ToShortDateString())
            )

        template()
            .MsgBox(alertBox)
            .DatepickerWidget(datePickerDoc)
            .SelectedDate(vDate)
            .Doc()
