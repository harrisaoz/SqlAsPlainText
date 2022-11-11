module SqlAsPlainText

open System.Data.SqlTypes
open FsHtmlKit
open Microsoft.SqlServer.Server

module Rtf = Rtf2PlainText
module Html = Html2PlainText

let chooseExtractor (fileExtension: string) =
    match fileExtension with
    | "" -> Rtf.tryExtractPlainText
    | ".rtf" -> Rtf.tryExtractPlainText
    | ".htm" -> Html.tryExtractPlainText
    | ".html" -> Html.tryExtractPlainText
    | _ -> id >> Some

[<SqlFunction>]
let asPlainText (fileExtension: SqlString) (docText: SqlString) : SqlString =
    docText.Value
    |> chooseExtractor fileExtension.Value
    |> function
        | None ->
            docText
        | Some plainText ->
            plainText |> SqlString
