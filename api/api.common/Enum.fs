module Djambi.Api.Common.Enum

open System

let inline getValues<'a>() : 'a list =
    (Enum.GetValues(typeof<'a>) :?> ('a []))
    |> Array.toList