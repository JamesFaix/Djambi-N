module Djambi.Api.Common.Nullable

open System

let ofValue<'a when 'a : struct
            and 'a :> ValueType
            and 'a : (new: Unit -> 'a)>
            (x : 'a) : 'a Nullable =
    new Nullable<'a>(x)