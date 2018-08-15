namespace Djambi.Api.Common

module Utilities =

    open System
    open System.Collections.Generic

    let inline GetValues<'a>() : 'a list =
        (Enum.GetValues(typeof<'a>) :?> ('a []))
        |> Array.toList

    let memoize fn =
        let cache = new Dictionary<_,_>()
        (fun input -> 
            match cache.TryGetValue input with  
            | true, output -> output
            | false, _ -> let output = fn(input)
                          cache.Add(input, output)
                          output)