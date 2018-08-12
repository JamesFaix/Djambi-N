module Utilities

open System

let inline GetValues<'a>() : 'a list =
    (Enum.GetValues(typeof<'a>) :?> ('a []))
    |> Array.toList
