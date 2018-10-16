module Djambi.Api.Web.Model.UserWebModel

[<CLIMutable>]
type CreateUserJsonModel =
    {
        name : string
        password : string
    }

[<CLIMutable>]
type UserResponseJsonModel =
    {
        id : int
        name : string
        isAdmin : bool
        //Don't return password here
    }        