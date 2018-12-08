[<AutoOpen>]
module Djambi.Api.Web.Mappings.SessionWebMapping

open Djambi.Api.Model
open Djambi.Api.Web.Model

let mapLoginRequestFromJson(jsonModel : LoginRequestJsonModel) : LoginRequest =
    {
        username = jsonModel.userName
        password = jsonModel.password
    }