module Djambi.Api.Web.Model.SessionWebModel

[<CLIMutable>]
type LoginRequestJsonModel =
    {
        userName : string
        password : string
    }