[<AutoOpen>]
module Djambi.Api.Common.Json.JsonUtility

open Newtonsoft.Json

let private converters = 
    [|
        OptionJsonConverter() :> JsonConverter
        UnionEnumJsonConverter() :> JsonConverter
        SingleFieldUnionJsonConverter() :> JsonConverter
    |]

let deserialize<'a> (json : string) : 'a =
    JsonConvert.DeserializeObject<'a>(json, converters)

let serialize<'a> (value : 'a) : string =
    JsonConvert.SerializeObject(value, converters)

let deserializeList<'a> (json : string) : 'a list =
    if json = null || json = ""
    then []
    else deserialize json

let deserializeOption<'a> (json : string) : 'a option =
    if json = null || json = ""
    then None
    else deserialize json