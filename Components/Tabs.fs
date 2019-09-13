namespace Components.Nav

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
module Tab = 

    type ActionType = 
        | Path of string
        | Callback of (unit->unit) 

    type NavTab = {
        Label : string
        Action : ActionType
        IsActive : bool
    }

    let private IsId (href:string) = 
        href.StartsWith "#"

    (* The Bootstrap's Tab component expects an Id at href attribute. This widget removes 
       such constraints to allow a URL path.
       It also supports a callback function
    *)
    let Tab (itemList:NavTab list) = 
        (* Output sample:
<ul class="nav nav-tabs">
  <li class="active"><a aria-expanded="true" href="#home" data-toggle="tab">Home</a></li>
  <li><a aria-expanded="false" href="#profile" data-toggle="tab">Profile</a></li>
  <li><a aria-expanded="false" href="#messages" data-toggle="tab">Messages</a></li>
  <li><a aria-expanded="false" href="#settings" data-toggle="tab">Settings</a></li>
</ul>
        *)

        let items = 
            itemList
            |> List.map (fun item ->
                let active,ariaOn = 
                    if item.IsActive then
                        "active","true"
                    else 
                        "","false"
                
                let linkElem =
                    match item.Action with
                    | Path path ->
                        let tabOn = 
                            if IsId path then 
                                "tab"
                            else
                                ""
                        a [ attr.href path
                            attr.``data-`` "toggle" tabOn
                            Attr.Create "aria-expanded" ariaOn
                          ] 
                          [ text item.Label ]

                    | Callback callback ->
                        a [ on.click (fun elem evt -> callback())
                            attr.href "javascript:void(0)"
                            attr.``data-`` "toggle" "tab"
                            Attr.Create "aria-expanded" ariaOn
                          ]
                          [ text item.Label ]

                li [ attr.``class`` active ] [ linkElem ]
            )


        ul [ attr.``class`` "nav nav-tabs" ] items 
       