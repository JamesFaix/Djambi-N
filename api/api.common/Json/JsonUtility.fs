[<AutoOpen>]
module Djambi.Api.Common.Json.JsonUtility

open System
open Newtonsoft.Json

let deserializeList<'a> (json : string) : 'a list =
    if json = null
    then List.empty
    else JsonConvert.DeserializeObject<'a list>(json)

let deserializeOption<'a> (json : string) : 'a option =
    if json = null
    then None
    else JsonConvert.DeserializeObject<'a option>(json)