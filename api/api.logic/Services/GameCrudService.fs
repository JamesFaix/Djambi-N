namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model
open Djambi.Api.Logic
open Djambi.Api.Enums
open FSharp.Control.Tasks
open System.Threading.Tasks

type GameCrudService(gameRepo : IGameRepository) =
    member x.createGame (parameters : GameParameters) (session : Session) : Task<Game> =
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

        task {
            let! gameId = gameRepo.createGameAndAddPlayer (gameRequest, playerRequest)
            return! gameRepo.getGame gameId
        }

    member x.getUpdateGameParametersEvent (game : Game, parameters : GameParameters) (session : Session) : CreateEventRequest =
        if game.status <> GameStatus.Pending
        then raise <| GameConfigurationException("Cannot change game parameters unless game is Pending.")
        Security.ensureCreatorOrEditPendingGames session game

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