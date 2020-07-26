namespace Apex.Api.Logic.Services

open System
open System.Linq
open Apex.Api.Common.Collections
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Model
open Apex.Api.Logic
open Apex.Api.Enums
open System.ComponentModel
open FSharp.Control.Tasks
open System.Threading.Tasks

type PlayerService(gameRepo : IGameRepository) =
    member __.getAddPlayerEvent (game : Game, request : CreatePlayerRequest) (session : Session) : CreateEventRequest =
        let self = session.user
        if game.status <> GameStatus.Pending
        then raise <| HttpException(400, "Can only add players to pending games.")
        elif request.name.IsSome
            && game.players |> List.exists (fun p -> String.Equals(p.name, request.name.Value, StringComparison.OrdinalIgnoreCase))
        then raise <| HttpException(409, "A player with that name already exists.")
        elif game.players.Length >= game.parameters.regionCount
        then raise <| HttpException(400, "Max player count reached.")
          else
            match request.kind with
            | PlayerKind.User ->
                if request.userId.IsNone
                then raise <| HttpException(400, "UserID must be provided when adding a user player.")
                elif request.name.IsSome
                then raise <| HttpException(400, "Cannot provide name when adding a user player.")
                elif game.players |> List.exists (fun p -> p.kind = PlayerKind.User && p.userId = request.userId)
                then raise <| HttpException(409, "User is already a player.")
                elif not (self.has Privilege.EditPendingGames) && request.userId.Value <> self.id
                then raise <| HttpException(403, "Cannot add other users to a game.")
                else ()

            | PlayerKind.Guest ->
                if not game.parameters.allowGuests
                then raise <| HttpException(400, "Game does not allow guest players.")
                elif request.userId.IsNone
                then raise <| HttpException(400, "UserID must be provided when adding a guest player.")
                elif request.name.IsNone
                then raise <| HttpException(400, "Must provide name when adding a guest player.")
                elif not (self.has Privilege.EditPendingGames) && request.userId.Value <> self.id
                then raise <| HttpException(403, "Cannot add guests for other users to a game.")
                else ()

            | PlayerKind.Neutral ->
                raise <| HttpException(400, "Cannot directly add neutral players to a game.")
            | _ -> raise <| InvalidEnumArgumentException()

        {
            kind = EventKind.PlayerJoined
            effects = [ PlayerAddedEffect.fromRequest request ]
            createdByUserId = self.id
            actingPlayerId = None
        }

    member __.getRemovePlayerEvent (game : Game, playerId : int) (session : Session) : CreateEventRequest =
        let self = session.user
        if game.status <> GameStatus.Pending then
            raise <| HttpException(400, "Cannot remove players unless game is Pending.")
        else
            match game.players |> List.tryFind (fun p -> p.id = playerId) with
            | None -> raise <| HttpException(404, "Player not found.")
            | Some player ->
                match player.userId with
                | None -> raise <| HttpException(400, "Cannot remove neutral players from game.")
                | Some x ->
                    if not <| (self.has Privilege.EditPendingGames
                        || game.createdBy.userId = self.id
                        || x = self.id)
                    then raise <| HttpException(403, "Cannot remove other users from game.")
                    else
                        let effects = new ArrayList<Effect>()

                        let playersToRemove =
                            match player.kind with
                            | PlayerKind.User ->
                                game.players
                                |> List.filter (fun p -> p.userId = player.userId)
                            | PlayerKind.Guest -> 
                                game.players
                                |> List.filter (fun p -> p.id = playerId)
                            | _ -> [] //Already eliminated this case in validation above

                        for p in playersToRemove do
                            effects.Add(Effect.PlayerRemoved { oldPlayer = p })

                        //Cancel game if creator quit
                        if game.createdBy.userId = player.userId.Value
                            && player.kind = PlayerKind.User
                        then
                            effects.Add(Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = GameStatus.Canceled })
                        else ()

                        {
                            kind = EventKind.PlayerRemoved
                            effects = effects |> Seq.toList
                            createdByUserId = self.id
                            actingPlayerId = Context.getActingPlayerId session game
                        }

    member __.fillEmptyPlayerSlots (game : Game) : Task<list<Effect>> =
        let missingPlayerCount = game.parameters.regionCount - game.players.Length

        let getNeutralPlayerNamesToUse (possibleNames : string list) =
            Enumerable.Except(
                possibleNames,
                game.players |> Seq.map (fun p -> p.name),
                StringComparer.OrdinalIgnoreCase)
            |> List.ofSeq
            |> List.shuffle
            |> Seq.take missingPlayerCount

        if missingPlayerCount = 0
        then Task.FromResult []
        else
            task {
                let! names = gameRepo.getNeutralPlayerNames()
                let namesToUse = getNeutralPlayerNamesToUse names
                let effects = namesToUse |> Seq.mapi (fun i name -> 
                    let placeholderPlayerId = -(i+1) //Use negative numbers to avoid conflicts
                    let f : NeutralPlayerAddedEffect = 
                        {
                            name = name
                            placeholderPlayerId = placeholderPlayerId
                        }
                    Effect.NeutralPlayerAdded f
                )
                return effects |> Seq.toList
            }