namespace Apex.Api.Logic.Services

open Apex.Api.Common.Collections
open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Db.Interfaces
open Apex.Api.Model
open Apex.Api.Logic

type GameCrudService(gameRepo : IGameRepository) =
    member x.createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
        match parameters.description with
        | Some x when not <| Validation.isValidGameDescription x ->
            errorTask <| HttpException(422, "Game descriptions cannot exceed 100 characters.")
        | _ -> 
            let self = session.user
            let gameRequest : CreateGameRequest =
                {
                    parameters = parameters
                    createdByUserId = self.id
                }

            let playerRequest : CreatePlayerRequest =
                {
                    kind = PlayerKind.User
                    userId = Some self.id
                    name = Some session.user.name
                }

            gameRepo.createGameAndAddPlayer (gameRequest, playerRequest)
            |> thenBindAsync gameRepo.getGame

    member x.getUpdateGameParametersEvent (game : Game, parameters : GameParameters) (session : Session) : CreateEventRequest HttpResult =
        if game.status <> GameStatus.Pending
        then Error <| HttpException (400, "Cannot change game parameters unless game is Pending.")
        else Ok ()
        |> Result.bind (fun _ -> Security.ensureCreatorOrEditPendingGames session game)
        |> Result.map (fun _ ->
            let effects = new ArrayList<Effect>()

            effects.Add(Effect.ParametersChanged { oldValue = game.parameters; newValue = parameters })

            //If lowering region count, extra players are ejected
            let truncatedPlayers =
                game.players
                |> Seq.skipSafe parameters.regionCount

            //If disabling AllowGuests, guests are ejected
            let ejectedGuests =
                if not parameters.allowGuests
                then game.players
                    |> Seq.filter (fun p -> p.kind = PlayerKind.Guest)
                else Seq.empty

            let removedPlayers =
                truncatedPlayers
                |> Seq.append ejectedGuests
                |> Seq.groupBy (fun p -> p.id)
                |> Seq.map (fun (_, elements) -> elements |> Seq.head)

            for player in removedPlayers do
                effects.Add(Effect.PlayerRemoved({oldPlayer = player}))

            {
                kind = EventKind.GameParametersChanged
                effects = effects |> Seq.toList
                createdByUserId = session.user.id
                actingPlayerId = Context.getActingPlayerId session game
            }
        )