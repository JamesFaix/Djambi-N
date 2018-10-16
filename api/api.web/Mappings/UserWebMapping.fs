module Djambi.Api.Web.Mappings.UserWebMapping

open Djambi.Api.Model.UserModel
open Djambi.Api.Web.Model.UserWebModel

let mapUserResponse(user : User) : UserResponseJsonModel =
    {
        id = user.id
        name = user.name
        isAdmin = user.isAdmin
    }

let mapCreateUserRequest(jsonModel : CreateUserJsonModel) : CreateUserRequest =
    {
        name = jsonModel.name
        password = jsonModel.password
    }