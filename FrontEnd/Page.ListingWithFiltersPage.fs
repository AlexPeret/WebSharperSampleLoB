namespace FrontEnd.Pages.Samples

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.Sitelets
open WebSharper.JavaScript
open WebSharper.Swiper

// remover
open WebSharper.UI.Templating

[<JavaScript>]
module ListingWithFiltersPage =

    open FrontEnd.Helpers
    open FrontEnd.Data
    open FrontEnd.Config
    open FrontEnd.Config.Route
    open Components.Forms.SelectBoxInput
    open Components.Forms.AutoCompleteInput
    open FrontEnd

    module Services = FrontEnd.Server

    type private template = Templating.Template<"templates/Page.ListingWithFilters.html">

    type Parameters = {
        Location : string
        MainOption : string option
        MinPrice : string option
        MaxPrice : string option
        Number : int option
    }

    let private mapToSelectItem (key,value):SelectItem<int> =
        { Key = key; Value = value }

    let private mapToAutoCompleteItem (key,value):AutoCompleteItem<string> =
        { Key = key; Value = value }

    (* Updates the model using values from URL *)
    let private SynchFilterModel (model:DTO.FilterOptions) (parameters:Parameters) =
        async {
            let mainOption =
                match parameters.MainOption with
                | None -> model.MainOption
                | Some mo ->
                    let optionO =
                        model.MainOptions
                        |> List.filter (fun option -> (fst >> string) option = mo)
                        |> List.tryHead
                    match optionO with
                    | None -> model.MainOption
                    | Some o -> o

            let! locationO = 
                Services.GetLocation parameters.Location

            let location =
                match locationO with
                | None -> model.Location
                | Some l -> l
                    
            // TODO: implement convertion validation
            let minPrice = Option.map float parameters.MinPrice
            let maxPrice = Option.map float parameters.MaxPrice
            let number = Option.map int parameters.Number

            return
                { model with
                    Location = location
                    MainOption = mainOption
                    MinPrice = minPrice
                    MaxPrice = maxPrice
                    Number = number }
        }

    let private MapParameters (vars:template.FilterBoxTemplate.Vars) location mainOption =
        {
            Location = location
            MainOption = mainOption
            MinPrice = StringToOption vars.MinimumPrice.Value
            MaxPrice = StringToOption vars.MaximumPrice.Value
            Number = None
        }

    let private SubmitFilter go filter =
        go (ListingWithFilters (filter.Location,filter.MainOption,filter.MinPrice,filter.MaxPrice,filter.Number))

    [<Require(typeof<UIResources.ListingWithFiltersSample.Css>)>]
    let private CreateCarrousel images =
        let afterRenderAttr = 
            on.afterRender(fun _ ->
                let swipeParams = 
                    SwipeParameters(
                        SpaceBetween = 10,
                        NextButton = Union2Of2 ".swiper-button-next",
                        PrevButton = Union2Of2 ".swiper-button-prev",
                        Parallax = true,
                        Speed = 600
                    )

                let gallery = 
                    new Swiper(".item-images", swipeParams)
                ()
            )
        let carrouselItems =
            images
            |> List.map (fun (i:string) ->
                template.CarrouselItem().ImagePath(i).Doc()
            )

        let carrousel =
            template.Carrousel().CarrouselItems(carrouselItems).Doc()

        div [ afterRenderAttr ] 
            [ carrousel ]

    let private MainOptionSelectBox go (model:DTO.FilterOptions) parameters rvOption =
        let options = 
            model.MainOptions
            |> List.map mapToSelectItem
                
        let rvListOptions = Var.Create options

        let selectBoxCallback (elem:SelectItem<int>) =
            let p = 
                { parameters with
                    MainOption = Some <| string elem.Key
                }
            SubmitFilter go p

        let mainOptionSelectBox =
            SelectBoxInputC "" selectBoxCallback rvOption rvListOptions

        mainOptionSelectBox
    
    let private LocationAutoComplete go parameters rvLocation =
        let autocompleteOnChange (elem:AutoCompleteItem<string>) =
            let p = 
                { parameters with
                    Location = elem.Key
                }
            SubmitFilter go p

        let config = 
            new AutoCompleteInputConfig<string,string>(
                Callback = Services.FindLocations,
                OnChangeCallback = autocompleteOnChange,
                DomID = "location",
                Placeholder = "type the location...",
                MinLength = 0
            )
        let locationComponent = 
            AutoCompleteInput rvLocation config

        locationComponent


    (* Remarks: 
        The filter box gets default values from the server or from URL, when available.

        - Parameters - The URL format:
          /listing-with-filters/location&main-option=?min-price=?max-price=?number=
        
        This page updates the URL accordingly to the filter options. In this case, 
        WebSharper will reload the page with the querystring parameters updated, 
        making the use of reactive variables unnecessary.
    *)
    let Main go (parameters:Parameters) =

        async {
        
            let! model =
                Services.DefaultFilterModel()
            let! model = SynchFilterModel model parameters

            let rvLocation = Var.Create <| mapToAutoCompleteItem model.Location
            let rvOption = Var.Create <| mapToSelectItem model.MainOption
            let rvMinimumPrice = Var.Create <| Helpers.AsOption "" string model.MinPrice
            let rvMaximumPrice = Var.Create <| Helpers.AsOption "" string model.MaxPrice

            // main options selectbox
            let mainOptionComponent =
                MainOptionSelectBox go model parameters rvOption
            let mainOptionKey = Some <| string rvOption.Value.Key

            // autocomplete field
            let locationComponent = 
                LocationAutoComplete go parameters rvLocation

            let locationKey = rvLocation.Value.Key

            let filterBox =
                template.FilterBoxTemplate()
                    .LocationComponent(locationComponent)
                    .MinimumPrice(rvMinimumPrice)
                    .OnMinimumPriceChange(fun t -> 
                        SubmitFilter go <| MapParameters t.Vars locationKey mainOptionKey)
                    .MaximumPrice(rvMaximumPrice)
                    .OnMaximumPriceChange(fun t -> 
                        SubmitFilter go <| MapParameters t.Vars locationKey mainOptionKey)
                    .MainOptionsSelectBox(mainOptionComponent)
                    .NumberOfOptionsGroupButtons(Doc.Empty)
                    .Doc()

            let! properties =
                Services.FindProperties model

            let listItems =
                properties
                |> List.map (fun p ->
                    let carrouselComponent = CreateCarrousel p.Images
                    template.Item()
                        .Images(carrouselComponent)
                        .Title(p.Title)
                        .Subtitle(p.Subtitle)
                        .Description(p.Description)
                        .Price(string p.Price)
                        .MoreInfoOnClick(fun t -> ())
                        .Doc()
                )
                
            return
                template()
                    .FilterBox(filterBox)
                    .Items(listItems)
                    .Doc()

        }
        |> Doc.Async

