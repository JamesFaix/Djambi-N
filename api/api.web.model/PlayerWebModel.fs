[<AutoOpen>]
module Djambi.Api.Web.Model.PlayerWebModel

open System

type PlayerResponseJsonModel =
    {
        id : int
        userId : int Nullable
        name : string
        ``type`` : string
    }

[<CLIMutable>]
type CreatePlayerJsonModel =
    {
        userId : int Nullable
        name : string
        ``type`` : string
    }