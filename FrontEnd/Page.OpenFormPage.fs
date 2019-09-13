namespace FrontEnd.Pages.Samples

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.JQueryUI
open WebSharper.JavaScript

[<JavaScript>]
module OpenFormPage =
    open Components.Forms.AutoCompleteInput
    open Components.Forms.SelectBoxInput
    open Components.Forms.DatePickerInput
    open Components.Display.Alert
    open Components.Display.LoadingGif
    open FrontEnd.Data.DTO
    open FrontEnd.Config
    open WebSharper.UI.Templating.Runtime.Server

    module Services = FrontEnd.Server

    type template = Templating.Template<"templates/Page.OpenForm.html">

    type PageStatus = {
        ShowStatus : Result<string,string list> -> unit
        ShowLoadingGif : unit -> unit
        HideLoadingGif : unit -> unit
    }

    let private MapToAutoCompleteItem (key,value):AutoCompleteItem<string> =
        { Key = key; Value = value }

    let private mapToSelectItem (key,value) =
        { Key = key; Value = value }

    let private LocationAutoComplete rvLocation =
        let config = 
            new AutoCompleteInputConfig<string,string>(
                Callback = Services.FindLocations,
                DomID = "location",
                Placeholder = "type the location...",
                MinLength = 0
            )

        AutoCompleteInput rvLocation config
    
    [<Require(typeof<FrontEnd.Config.UIResources.JQueryUIDatePickerPtBr.Js>)>]
    let Main go eId =

        let rvAlert = Var.Create (Ok "")
        let alertElem = AlertResult rvAlert

        let rvLoading = Var.Create false
        let loadingGifElem = LoadingGif rvLoading

        // helper functions
        let pageStatus = {
            ShowStatus = fun res -> Var.Set rvAlert res
            ShowLoadingGif  = fun () -> Var.Set rvLoading true
            HideLoadingGif = fun () -> Var.Set rvLoading false
        }

        let loadModel eId =
            async {
                pageStatus.ShowLoadingGif()

                let! model = Services.GetAccountInfo eId

                pageStatus.HideLoadingGif()
                return model
            }


        async {
            let! model = loadModel eId

            let rvLocation = Var.CreateWaiting ()
            let rvEmail =  Var.CreateWaiting ()
            let rvGender =  Var.CreateWaiting ()
            let rvGenders =  Var.CreateWaiting ()
            let rvBirthday =  Var.CreateWaiting ()

            let modelToForm (model:AccountModel) =
                Var.Set rvLocation <| MapToAutoCompleteItem model.Location
                Var.Set rvEmail model.Email
                Var.Set rvGender <| mapToSelectItem model.Gender
                Var.Set rvGenders <| (model.Genders |> List.map mapToSelectItem)
                Var.Set rvBirthday <| CreateMaybeDate model.Birthday

            let formToModel birthday =
                {
                    Id = eId
                    Location = rvLocation.Value.Key,rvLocation.Value.Value
                    Email = rvEmail.Value
                    Genders = []
                    Gender = rvGender.Value.Key,rvGender.Value.Value
                    Birthday = birthday
                }
            
            let OnBackCallback (t:TemplateEvent<template.Vars,Dom.MouseEvent>) =
                go Route.EndPoint.SamplePages

            let OnSaveCallback (t:TemplateEvent<template.Vars,Dom.MouseEvent>) = 
                async {
                    pageStatus.ShowLoadingGif()

                    // make sure the birthday date is valid
                    let resModel =
                        match rvBirthday.Value with
                        | InvalidDate _ -> Result.Error [ "invalid date format" ]
                        | ValidDate date -> 
                            Ok <| formToModel date
                    
                    let! res = 
                        match resModel with
                        | Result.Error msgs ->
                            async {
                                return Result.Error msgs
                            }
                        | Ok model ->
                            Services.SaveAccountInfo model

                    pageStatus.ShowStatus res

                    match res with
                    | Error _ -> ()
                    | Ok _ -> 
                        // reload from database. Uncomment once implemented
                        let! model = loadModel eId
                        modelToForm model

                    pageStatus.HideLoadingGif()
                }
                |> Async.Start


            let locationFieldComponent =
                LocationAutoComplete rvLocation

            let genderFieldComponent = 
                SelectBoxInput "" rvGender rvGenders
            
            let config =
                DatepickerConfiguration(
                    DateFormat = "dd/mm/yy",
                    ChangeMonth = true,
                    ChangeYear = true,
                    AutoSize = true
                )

            let birthdayFieldComponent =
                DatePickerInput rvBirthday config

            modelToForm model

            return 
                template()
                    .StatusLoading(loadingGifElem)
                    .StatusMsg(alertElem)
                    .LocationComponent(locationFieldComponent)
                    .Email(rvEmail)
                    .GenderComponent(genderFieldComponent)
                    .BirthdayComponent(birthdayFieldComponent)
                    .OnBack(OnBackCallback)
                    .OnSave(OnSaveCallback)
                    .Doc()
        }
        |> Doc.Async
