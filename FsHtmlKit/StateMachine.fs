module FsHtmlKit.StateMachine

open FSharp.Data
open FSharp.Data.HtmlActivePatterns

module StringExt = FsCombinators.StringExtensions

open BufferManipulation
open HtmlRules.Space
open HtmlRules.Breaking
open HtmlRules.Headings

type CollapseState =
    | Init
    | InSpace
    | InWord

let initialState: string * CollapseState = ("", Init)

let carriageReturn (buffer: string, _) =
    match String.length buffer with
    | n when n > 0 && (not (buffer |> StringExt.endsWith newline)) ->
        (appendNewline buffer, Init)
    | _ ->
        (buffer, Init)

let collapse text (buffer, state) =
    let transitionOnCharacter (buffer', state') character =
        if (isCollapsible character) then
            match state' with
            | Init ->
                (buffer', Init)
            | _ ->
                (buffer', InSpace)
        else
            let appendCharTo = StringExt.appendString (sprintf "%c" character)
            match state' with
            | InSpace ->
                (appendSpace buffer' |> appendCharTo, InWord)
            | _ ->
                (buffer' |> appendCharTo, InWord)

    match String.length text with
    | 0 -> (buffer, state)
    | _ ->
        text
        |> Seq.fold transitionOnCharacter (buffer, state)

let visitNode transform (buffer, state) (node: HtmlNode): string * CollapseState =
    match node with
    | HtmlElement(name, _, _) when (isHeading name || isBreakingElement name) ->
        carriageReturn (buffer, state)
    | HtmlText textValue when textValue.Equals(newline) ->
        carriageReturn (buffer, state)
    | _ ->
        collapse (transform node) (buffer, state)

let runMachine transform =
    Seq.fold (visitNode transform) initialState
