module Djambi.Api.Common.Utilities

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
 

let nullableToOption<'a when 'a : struct
                and 'a :> ValueType
                and 'a : (new: Unit -> 'a)>
                (x : 'a Nullable) : 'a option =
    match x with
    | some when some.HasValue -> Some some.Value
    | _ -> None

let referenceToOption<'a when 'a : null> (x : 'a) : 'a option =
    match x with
    | null -> None
    | _ as value -> Some value

let stringToOption (str : string) : string option =
    match str with
    | null
    | "" -> None
    | _ as value -> Some value;

let optionToNullable<'a when 'a : struct
                and 'a :> ValueType
                and 'a : (new: Unit -> 'a)>
                (x : 'a option) : 'a Nullable =
    match x with
    | Some value -> new Nullable<'a>(value)
    | None -> new Nullable<'a>()

let optionToReference<'a when 'a : null> (x : 'a option) : 'a =
    match x with
    | None -> null
    | Some value -> value

let toNullable<'a when 'a : struct
            and 'a :> ValueType
            and 'a : (new: Unit -> 'a)>
            (x : 'a) : 'a Nullable =
    new Nullable<'a>(x)