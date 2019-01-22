[<AutoOpen>]
module Djambi.Api.Common.Json.JsonUtility

open Newtonsoft.Json

let deserializeList<'a> (json : string) : 'a list =
    if json = null || json = ""
    then List.empty
    else JsonConvert.DeserializeObject<'a list>(json)

let deserializeOption<'a> (json : string) : 'a option =
    if json = null || json = ""
    then None
    else JsonConvert.DeserializeObject<'a option>(json)