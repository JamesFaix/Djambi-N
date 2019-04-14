module Djambi.Api.Common.Collections

type ArrayList<'a> = System.Collections.Generic.List<'a>

module List =

    open System

    let private random = new Random()

    let shuffle<'a> (xs : 'a list) : 'a list =
        let list = new ArrayList<'a>(xs)
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

    let rotate<'a> (places : int) (xs : 'a list) : 'a list =
        let mutable xs = xs
        for _ in [1..places] do
            xs <- List.append (xs |> List.tail) [(xs |> List.head)]
        xs

module Seq =
    let replaceIf<'a> (predicate : 'a -> bool) (replace : 'a -> 'a) (list : 'a seq) : 'a seq =
        list
        |> Seq.map (fun x ->
            if predicate x
            then replace x
            else x
        )

    let exceptWithKey<'a, 'key when 'key : equality> (keySelector : 'a -> 'key) (keys : 'key seq) (xs : 'a seq) : 'a seq =
        let keyList = keys |> Seq.toList
        xs
        |> Seq.filter (fun x ->
            let key = keySelector x
            keyList |> List.contains key |> not
        )

    let skipSafe<'a> (count : int) (xs : 'a seq) : 'a seq =
        use e = xs.GetEnumerator()
        let mutable n = 0
        seq {
            while e.MoveNext() do
                n <- n+1
                if n > count then
                    yield e.Current
        }

    let values<'a> (xs : 'a option seq) : 'a seq =
        xs
        |> Seq.filter Option.isSome
        |> Seq.map (fun o -> o.Value)