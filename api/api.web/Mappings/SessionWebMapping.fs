module Djambi.Api.Web.Mappings.SessionWebMapping

open Djambi.Api.Model.SessionModel
open Djambi.Api.Web.Model.SessionWebModel

let mapLoginRequestFromJson(jsonModel : LoginRequestJsonModel) : LoginRequest =
    {
        username = jsonModel.userName
        password = jsonModel.password
    }