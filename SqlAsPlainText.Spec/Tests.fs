module Tests

open System.Data.SqlTypes
open FsCheck
open FsCheck.Xunit

open Giraffe.ViewEngine

open FsCombinators
open SqlAsPlainText

open SqlAsPlainText.Spec.Examples

module HtmlText =
    type T = T of string

    let allValidChars xs =
        let isValidChar (ch: char) =
            [ System.Char.IsLetterOrDigit
              System.Char.IsPunctuation
              System.Char.IsSeparator ]
            |> SeqExtensions.existsPredicate ch

        Seq.forall isValidChar xs

    let create (s: string) =
        let isOk (s: string) =
            String.length s < 1000 && allValidChars s

        match s with
        | null -> None
        | nonNull when isOk nonNull -> nonNull.Trim() |> T |> Some
        | _ -> None

    let apply f (T s) = f s
    let value s = apply id s

let htmlEncode text =
    RenderView.AsString.htmlNode <| str text

let arbHtmlText =
    Arb.Default.String().Generator
    |> Gen.map HtmlText.create
    |> Gen.filter Option.isSome
    |> Arb.fromGen

let initialTextMatches example =
    Prop.forAll arbHtmlText
    <| fun m ->
        let paraText =
            Option.get m |> HtmlText.value

        let plainText =
            example paraText
            |> SqlString
            |> asPlainText (SqlString ".html")

        plainText.Value.StartsWith(paraText)
        |> Prop.collect paraText.Length

[<Property>]
let ``Body-less HTML snippet`` () = initialTextMatches example1

[<Property>]
let ``Headless HTML document`` () = initialTextMatches example2

[<Property>]
let ``Normal HTML document`` () = initialTextMatches example3
