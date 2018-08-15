namespace Djambi.Api.Common

module Utilities =

    open System
    open System.Collections.Generic
    open System.Linq

    let private random = new Random()

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

    let shuffle<'a> (xs : 'a seq) : 'a seq =
        let list = xs.ToList()
        let mutable n = list.Count
        while (n > 1) do
            n <- n - 1
            let k = random.Next(n + 1)
            let value = list.[k]
            list.[k] <- list.[n]
            list.[n] <- value
        list :> 'a seq