module FsHtmlKit.Html2PlainText

open FSharp.Data

module SM = StateMachine
module PT = AsPlainText
module E = Enumeration

let tryExtractPlainText (docText: string) =
    let elements _ doc : HtmlNode seq =
        HtmlDocumentExtensions.Elements doc

    docText
    |> Parse.tryParse
    |> Option.fold elements Seq.empty
    |> Seq.collect (E.filteredDescendents PT.elementsAndTextOnly)
    |> SM.runMachine PT.asPlainText
    |> fst
    |> function
        | "" -> None
        | text -> Some text

let tryExtractPlainTextOfBody (docText: string) =
    docText
    |> Parse.tryParse
    |> Option.bind HtmlDocumentExtensions.TryGetBody
    |> Option.map (
        E.filteredDescendents PT.elementsAndTextOnly
        >> SM.runMachine PT.asPlainText)
    |> Option.map fst

let html2Text (docText: string) =
    docText
    |> tryExtractPlainText
    |> function
        | None -> docText
        | Some buffer -> buffer
