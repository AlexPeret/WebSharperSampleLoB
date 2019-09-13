namespace FrontEnd.Pages.Samples

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.JQueryUI
open WebSharper.JavaScript

[<JavaScript>]
module ClosedFormPage =
    open Components.Forms.AutoCompleteInput
    open Components.Forms.SelectBoxInput
    open Components.Forms.DatePickerInput
    open Components.Display.Alert
    open Components.Display.LoadingGif
    open FrontEnd.Data.DTO

    module Services = FrontEnd.Server

    type template = Templating.Template<"templates/Page.ClosedForm.html">

    type PageModel = {
        EId: int64
        // for data restore on cancelling
        OriginalModel: AccountModel option
        Model: AccountModel option
        // for each editable field
        ShowLocationField: bool
        ShowEmailField: bool
        ShowGenderField: bool
        ShowBirthdayField: bool
        Location: AutoCompleteItem<string>
        Email: string
        Gender: SelectItem<int>
        Genders: SelectItem<int> list
        Birthday: MaybeDate
    }

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
    
    let private BuildField (rvShowField:Var<bool>) (readonlyContent:Doc) 
        (editableContent:Doc) OnCancelCallback OnSaveCallback OnEditCallback =
        let readonlyField =
            template.ReadonlyField()
                .OnEdit(OnEditCallback)
                .ReadonlyContent(readonlyContent)
                .Doc()

        let editField =
            template.EditField()
                .OnCancel(OnCancelCallback)
                .OnSave(OnSaveCallback)
                .EditContent(editableContent)
                .Doc()

        let fieldComponent =
            rvShowField.View
            |> View.Map(fun show ->
                if show then
                    editField
                else
                    readonlyField
            )
            |> Doc.EmbedView

        fieldComponent


    let private BuildLocationField (rvPageModel:Var<PageModel>)
        rvLocation pageStatus loadModel modelToForm =
        let locationInput =
            LocationAutoComplete rvLocation
        let locationLabel =
            label [] [ textView <| rvLocation.View.Map(fun l -> l.Value) ]
            
        let rvShowField = 
            rvPageModel.Lens (fun p -> p.ShowLocationField)
                             (fun p n -> { p with ShowLocationField = n })
        
        let OnCancelCallback t =
            match rvPageModel.Value.OriginalModel with
            | None -> ()
            | Some originalModel -> modelToForm <| originalModel

            Var.Set rvShowField false

        let OnSaveCallback t =
            async {
                pageStatus.ShowLoadingGif()

                let elem = Var.Get rvLocation
                let location = (elem.Key,elem.Value)
                let! res = 
                    match rvPageModel.Value.Model with
                    | None -> async { return (Result.Error [ "unexpecated form state" ]) }
                    | Some model ->
                        Services.SaveAccountInfo 
                            { model with Location = location }

                pageStatus.ShowStatus res

                match res with
                | Error _ -> ()
                | Ok _ -> 
                    // reload from database. Uncomment once implemented
                    //let! model = loadModel (rvPageModel.Value.EId)
                    //modelToForm model
                    Var.Set rvShowField false

                pageStatus.HideLoadingGif()
            }
            |> Async.Start

        let OnEditCallback t =
            Var.Set rvShowField true

        BuildField rvShowField locationLabel locationInput
            OnCancelCallback OnSaveCallback OnEditCallback 

    let private BuildEmailField (rvPageModel:Var<PageModel>)
        rvEmail pageStatus loadModel modelToForm =
        let emailInput = Doc.Input [] rvEmail
        let emailLabel =
            label [] [ textView <| rvEmail.View.Map(fun l -> l) ]
            
        let rvShowField = 
            rvPageModel.Lens (fun p -> p.ShowEmailField)
                             (fun p n -> { p with ShowEmailField = n })
        
        let OnCancelCallback t =
            match rvPageModel.Value.OriginalModel with
            | None -> ()
            | Some originalModel -> modelToForm <| originalModel

            Var.Set rvShowField false

        let OnSaveCallback t =
            async {
                pageStatus.ShowLoadingGif()

                let email = Var.Get rvEmail
                let! res = 
                    match rvPageModel.Value.Model with
                    | None -> async { return (Result.Error [ "unexpecated form state" ]) }
                    | Some model ->
                        Services.SaveAccountInfo 
                            { model with Email = email }

                pageStatus.ShowStatus res

                match res with
                | Error _ -> ()
                | Ok _ -> 
                    // reload from database. Uncomment once implemented
                    //let! model = loadModel (rvPageModel.Value.EId)
                    //modelToForm model
                    Var.Set rvShowField false

                pageStatus.HideLoadingGif()
            }
            |> Async.Start

        let OnEditCallback t =
            Var.Set rvShowField true

        BuildField rvShowField emailLabel emailInput
            OnCancelCallback OnSaveCallback OnEditCallback 

    let private BuildGenderField (rvPageModel:Var<PageModel>)
        rvGender rvGenders pageStatus loadModel modelToForm =
        let genderInput = 
            SelectBoxInput "" rvGender rvGenders
        let genderLabel =
            label [] [ textView <| rvGender.View.Map(fun l -> l.Value) ]
            
        let rvShowField = 
            rvPageModel.Lens (fun p -> p.ShowGenderField)
                             (fun p n -> { p with ShowGenderField = n })
        
        let OnCancelCallback t =
            match rvPageModel.Value.OriginalModel with
            | None -> ()
            | Some originalModel -> modelToForm <| originalModel

            Var.Set rvShowField false

        let OnSaveCallback t =
            async {
                pageStatus.ShowLoadingGif()

                let gender = Var.Get rvGender
                let! res = 
                    match rvPageModel.Value.Model with
                    | None -> async { return (Result.Error [ "unexpecated form state" ]) }
                    | Some model ->
                        Services.SaveAccountInfo 
                            { model with Gender = gender.Key,gender.Value }

                pageStatus.ShowStatus res

                match res with
                | Error _ -> ()
                | Ok _ -> 
                    // reload from database. Uncomment once implemented
                    //let! model = loadModel (rvPageModel.Value.EId)
                    //modelToForm model
                    Var.Set rvShowField false

                pageStatus.HideLoadingGif()
            }
            |> Async.Start

        let OnEditCallback t =
            Var.Set rvShowField true

        BuildField rvShowField genderLabel genderInput
            OnCancelCallback OnSaveCallback OnEditCallback 

    [<Require(typeof<FrontEnd.Config.UIResources.JQueryUIDatePickerPtBr.Js>)>]
    let private BuildBirthdayField (rvPageModel:Var<PageModel>)
        rvBirthday pageStatus loadModel modelToForm =
        let config =
            DatepickerConfiguration(
                DateFormat = "dd/mm/yy",
                ChangeMonth = true,
                ChangeYear = true,
                AutoSize = true
            )

        let birthdayInput = 
            DatePickerInput rvBirthday config
        let vBirthday =
            rvBirthday.View
            |> View.Map(fun date ->
                match date with
                | InvalidDate msg -> msg
                | ValidDate dateO -> 
                    match dateO with
                    | None -> ""
                    | Some d -> d.ToShortDateString()
            )
        let birthdayLabel =
            label [] [ textView <| vBirthday ]
            
        let rvShowField = 
            rvPageModel.Lens (fun p -> p.ShowBirthdayField)
                             (fun p n -> { p with ShowBirthdayField = n })
        
        let OnCancelCallback t =
            match rvPageModel.Value.OriginalModel with
            | None -> ()
            | Some originalModel -> modelToForm <| originalModel

            Var.Set rvShowField false

        let OnSaveCallback t =
            async {
                pageStatus.ShowLoadingGif()

                let birthday = Var.Get rvBirthday
                let model =
                    match rvPageModel.Value.Model with
                    | None -> Result.Error [ "unexpecated form state" ]
                    | Some model ->
                        match birthday with
                        | InvalidDate msg -> Result.Error [ sprintf "invalid date: %s" msg ]
                        | ValidDate dateO ->
                            Ok { model with Birthday = dateO }

                let! res = 
                    match model with
                    | Result.Error msg -> async { return (Result.Error msg) }
                    | Ok model' ->
                        Services.SaveAccountInfo model'

                pageStatus.ShowStatus res

                match res with
                | Error _ -> ()
                | Ok _ -> 
                    // reload from database. Uncomment once implemented
                    //let! model = loadModel (rvPageModel.Value.EId)
                    //modelToForm model
                    Var.Set rvShowField false

                pageStatus.HideLoadingGif()
            }
            |> Async.Start

        let OnEditCallback t =
            Var.Set rvShowField true

        BuildField rvShowField birthdayLabel birthdayInput
            OnCancelCallback OnSaveCallback OnEditCallback 


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

        let rvPageModel = 
            Var.Create
                {
                    EId = eId
                    OriginalModel = None
                    Model = None
                    ShowLocationField = false
                    ShowEmailField = false
                    ShowGenderField = false
                    ShowBirthdayField = false
                    Location = { Key = ""; Value = "" }
                    Email = ""
                    Gender = (mapToSelectItem (0,""))
                    Genders = []
                    Birthday = ValidDate None
                }        

        let loadModel eId =
            async {
                pageStatus.ShowLoadingGif()

                let! model = Services.GetAccountInfo eId
                Var.Set rvPageModel
                    { rvPageModel.Value with 
                        OriginalModel = Some model
                        Model = Some model }

                pageStatus.HideLoadingGif()
                return model
            }

        async {
            let! model = loadModel eId

            let rvLocation = 
                rvPageModel.Lens (fun p -> p.Location)
                                 (fun p n -> { p with Location = n })
            
            let rvEmail = 
                rvPageModel.Lens (fun p -> p.Email)
                                 (fun p n -> { p with Email = n })

            let rvGender = 
                rvPageModel.Lens (fun p -> p.Gender)
                                 (fun p n -> { p with Gender = n })

            let rvGenders = 
                rvPageModel.Lens (fun p -> p.Genders)
                                 (fun p n -> { p with Genders = n })

            let rvBirthday = 
                rvPageModel.Lens (fun p -> p.Birthday)
                                 (fun p n -> { p with Birthday = n })

            let modelToForm (model:AccountModel) =
                Var.Set rvLocation <| MapToAutoCompleteItem model.Location
                Var.Set rvEmail model.Email
                Var.Set rvGender <| mapToSelectItem model.Gender
                Var.Set rvGenders <| (model.Genders |> List.map mapToSelectItem)
                Var.Set rvBirthday <| CreateMaybeDate model.Birthday

            let locationFieldComponent =
                BuildLocationField rvPageModel rvLocation pageStatus
                    loadModel modelToForm

            let emailFieldComponent =
                BuildEmailField rvPageModel rvEmail pageStatus
                    loadModel modelToForm

            let genderFieldComponent = 
                BuildGenderField rvPageModel rvGender rvGenders pageStatus
                    loadModel modelToForm
            
            let birthdayFieldComponent =
                BuildBirthdayField rvPageModel rvBirthday pageStatus
                    loadModel modelToForm

            let formFields = [
                locationFieldComponent
                emailFieldComponent
                genderFieldComponent
                birthdayFieldComponent
            ]

            modelToForm model

            return 
                template()
                    .StatusLoading(loadingGifElem)
                    .StatusMsg(alertElem)
                    .Fields(formFields)
                    .Doc()
        }
        |> Doc.Async
