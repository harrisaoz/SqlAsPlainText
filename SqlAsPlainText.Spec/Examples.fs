module SqlAsPlainText.Spec.Examples

open Giraffe.ViewEngine

let example1 divText =
    [ meta
          [ _httpEquiv "Content-Type"
            _content "text/html"
            _charset "utf-8" ]
      div [] [ str divText ] ]
    |> RenderView.AsString.htmlNodes

let example2 hText =
    html [] [ body [] [ h1 [] [ str hText ] ] ]
    |> RenderView.AsString.htmlNode

let example3 spanText =
    html
        []
        [ head [] [ title [] [ str "Title" ] ]
          body [] [ p [] [ span [] [ str spanText ] ] ] ]
    |> RenderView.AsString.htmlNode
