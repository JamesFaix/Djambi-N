module Apex.Api.Common.Option

open System

let ofNullable<'a when 'a : struct
                and 'a :> ValueType
                and 'a : (new: Unit -> 'a)>
                (x : 'a Nullable) : 'a option =
    match x with
    | some when some.HasValue -> Some some.Value
    | _ -> None

let ofReference<'a when 'a : null> (x : 'a) : 'a option =
    match x with
    | null -> None
    | value -> Some value

let ofString (str : string) : string option =
    match str with
    | null
    | "" -> None
    | value -> Some value;

let toNullable<'a when 'a : struct
                and 'a :> ValueType
                and 'a : (new: Unit -> 'a)>
                (x : 'a option) : 'a Nullable =
    match x with
    | Some value -> new Nullable<'a>(value)
    | None -> new Nullable<'a>()

let toReference<'a when 'a : null> (x : 'a option) : 'a =
    match x with
    | None -> null
    | Some value -> value
