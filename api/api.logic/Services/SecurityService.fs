namespace Djambi.Api.Logic.Services

open Djambi.Api.Model

type SecurityService() =
    member x.isGameViewableByActiveUser (session : Session) (game : Game) : bool =
        let self = session.user
        game.parameters.isPublic
        || game.createdBy.userId = self.id
        || game.players |> List.exists(fun p -> p.userId = Some self.id)
