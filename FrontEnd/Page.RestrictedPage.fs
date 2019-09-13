namespace FrontEnd.Pages

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html

[<JavaScript>]
module RestrictedPage =

    let Main go =
        div []
            [ text "Private page. If you can see this, your are logged." ]

    let NoAccess go =
        div []
            [ text "Access Denied! you must get logged to access this content" ]
