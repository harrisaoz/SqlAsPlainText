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

/// Extract plain text from the provided document text, using an extraction
/// strategy based on the given file extension.
/// Inputs and outputs are compatible with nvarchar(N), where N may be
/// either a numeric value, or the special symbol, 'max'.
/// It is expected that the docText input and the return value will correspond
/// to nvarchar(max) database column types, whereas the fileExtension value
/// is likely to correspond to a nvarchar(N) value where N < 30.
[<SqlFunction>]
let asPlainText (fileExtension: SqlString) (docText: SqlString) : SqlString =
    docText.Value
    |> chooseExtractor fileExtension.Value
    |> function
        | None ->
            docText
        | Some plainText ->
            plainText |> SqlString
