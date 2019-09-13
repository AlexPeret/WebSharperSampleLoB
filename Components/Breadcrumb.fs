namespace Components.Nav

open System
open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html

[<JavaScript>]
module Breadcrumb = 
    open WebSharper

    // discriminated union
    type ActionType = 
        | Path of string
        | Callback of (unit->unit) 

    type NavItem = {
        Label : string
        Action : ActionType
    }

    let Breadcrumb (itemList:NavItem list) = 
        (*
<ol class="breadcrumb">
  <li><a href="/breadcrumb" >Home</a></li>
  <li><a href="javascript:void(0)" script="fnCallback()">Library</a></li>
  <li class="active">Data</li>
</ol>        
        *)
        let count = List.length itemList

        let items = 
            itemList
            |> List.mapi(fun idx item ->
                let active = 
                    if idx=count-1 then
                        attr.``class`` "active"
                    else 
                        attr.``class`` ""

                let linkElem =
                    match item.Action with
                    | Path path ->
                        a [ attr.href path ] [ text item.Label ]
                    | Callback callback ->
                        a [ on.click (fun elem evt -> callback()); attr.href "javascript:void(0)" ] [ text item.Label ]    
                li [ active ] [ linkElem ]
            )

        ol [ attr.``class`` "breadcrumb" ] items 
       