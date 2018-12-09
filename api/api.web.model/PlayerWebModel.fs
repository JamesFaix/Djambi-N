[<AutoOpen>]
module Djambi.Api.Web.Model.PlayerWebModel

open Djambi.Api.Model

type PlayerResponseJsonModel =
    {
        id : int
        userId : int option
        name : string
        ``type`` : PlayerType
    }

[<CLIMutable>]
type CreatePlayerJsonModel =
    {
        userId : int option
        name : string option
        ``type`` : PlayerType
    }