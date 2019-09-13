namespace FrontEnd.Pages

open WebSharper
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.Remarkable

[<JavaScript>]
module Help =
    open FrontEnd.Server

    type private template = Templating.Template<"templates/Page.Help.html">

    let Main () =

        let config = new Options()
        config.Linkify <- true
        config.Typographer <- false

        let Md = new Remarkable(config)

        async {
            let! docsRows = HelpContent ()

            let newLine = "\r\n"

            (* The logic below makes sure all code blocks are marked at once,
                avoiding multiple <pre><code>...</code><pre> blocks. *)
            let codeBlockIndexes =
                docsRows
                |> List.indexed
                |> List.where(fun (i,e) -> e = "```")
                |> List.map (fun (i,e) -> i)

            let codeBlockIndexesRanges =
                codeBlockIndexes
                |> List.pairwise
                |> List.indexed
                |> List.filter (fun (indice,_) ->
                    let somentePar = (indice % 2) = 0
                    somentePar
                )
                |> List.map (fun (_,elem) -> elem)

            let docsRowsWithFoldCodeBlocks' = 
                codeBlockIndexesRanges
                |> List.rev
                |> List.fold
                    (fun (remainingList,newList) (iStart,iEnd) -> 
                        let afterBlock = 
                            remainingList
                            |> List.skip (iEnd + 1)
                        let blockCodeAsOneLine = 
                            remainingList
                            |> List.skip iStart
                            |> List.take (iEnd - iStart + 1)
                            |> List.fold (fun accc elem -> accc + newLine + elem) ""
                        
                        let beforeBlock = 
                            remainingList
                            |> List.take iStart
                        beforeBlock,[blockCodeAsOneLine]@afterBlock@newList
                        
                    ) 
                    (docsRows,[])

            let docsRowsWithFoldCodeBlocks = 
                (fst docsRowsWithFoldCodeBlocks')@(snd docsRowsWithFoldCodeBlocks')

            let docsHtml = 
                docsRowsWithFoldCodeBlocks
                |> List.map (fun row -> Md.Render row)
                |> List.fold (fun acc htmlRow -> acc + newLine + htmlRow) ""

            let docsMarkdown = Doc.Verbatim docsHtml 

            return template()
                  .Documentacao(docsMarkdown)
                  .Doc()
        }
        |> Doc.Async
