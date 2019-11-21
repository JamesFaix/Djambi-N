module Apex.Api.Common.Function

open System.Collections.Generic

let memoize fn =
    let cache = new Dictionary<_,_>()
    (fun input ->
        match cache.TryGetValue input with
        | true, output -> output
        | false, _ -> let output = fn(input)
                      cache.Add(input, output)
                      output)
