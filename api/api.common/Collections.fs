module Djambi.Api.Common.Collections

type ArrayList<'a> = System.Collections.Generic.List<'a>

module List =

    open System
    open System.Linq

    let private random = new Random()

    let shuffle<'a> (xs : 'a list) : 'a list =
        let list = xs.ToList()
        let mutable n = list.Count
        while (n > 1) do
            n <- n - 1
            let k = random.Next(n + 1)
            let value = list.[k]
            list.[k] <- list.[n]
            list.[n] <- value
        List.ofSeq list

    let replaceIf<'a> (predicate : 'a -> bool) (replace : 'a -> 'a) (list : 'a list) : 'a list =
        list
        |> List.map (fun x -> 
            if predicate x
            then replace x
            else x
        )

    let exceptWithKey<'a, 'key when 'key : equality> (keySelector : 'a -> 'key) (keyList : 'key list) (list : 'a list) : 'a list =
        list 
        |> List.filter (fun x -> 
            let key = keySelector x
            keyList |> List.contains key |> not
        )

    let skipSafe<'a> (count : int) (xs : 'a list) : 'a list =
        if count > xs.Length
        then List.empty
        else xs |> List.skip count