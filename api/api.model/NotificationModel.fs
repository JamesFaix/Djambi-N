[<AutoOpen>]
module Djambi.Api.Model.NotificationModel

type SubscriberId =
    {
        userId : int
        gameId : int option
    }