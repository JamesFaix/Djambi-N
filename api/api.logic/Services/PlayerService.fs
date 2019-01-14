module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

type ArrayList<'a> = System.Collections.Generic.List<'a>
    
//TODO: Add integration tests
let getAddPlayerEvent (game : Game, request : CreatePlayerRequest) (session : Session) : Event HttpResult =
    if game.status <> GameStatus.Pending
    then Error <| HttpException(400, "Can only add players to pending games.")
    elif request.name.IsSome 
        && game.players |> List.exists (fun p -> String.Equals(p.name, request.name.Value, StringComparison.OrdinalIgnoreCase))
    then Error <| HttpException(409, "A player with that name already exists.")
    elif game.players.Length >= game.parameters.regionCount
    then Error <| HttpException(400, "Max player count reached.")
      else
        match request.kind with
        | PlayerKind.User ->
            if request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a user player.")
            elif request.name.IsSome
            then Error <| HttpException(400, "Cannot provide name when adding a user player.")
            elif game.players |> List.exists (fun p -> p.kind = PlayerKind.User && p.userId = request.userId)
            then Error <| HttpException(409, "User is already a player.")
            elif not session.isAdmin && request.userId.Value <> session.userId
            then Error <| HttpException(403, "Cannot add other users to a game.")
            else Ok ()

        | PlayerKind.Guest ->
            if not game.parameters.allowGuests
            then Error <| HttpException(400, "Game does not allow guest players.")
            elif request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a guest player.")
            elif request.name.IsNone
            then Error <| HttpException(400, "Must provide name when adding a guest player.")
            elif not session.isAdmin && request.userId.Value <> session.userId
            then Error <| HttpException(403, "Cannot add guests for other users to a game.")
            else Ok ()

        | PlayerKind.Neutral ->
            Error <| HttpException(400, "Cannot directly add neutral players to a game.")
    |> Result.map (fun _ -> Event.create(EventKind.PlayerJoined, [Effect.playerAdded request]))

//TODO: Add integration tests
let getRemovePlayerEvent (game : Game, playerId : int) (session : Session) : Event HttpResult =
    match game.status with
    | Aborted | AbortedWhilePending | Finished -> 
        Error <| HttpException(400, "Cannot remove players from finished or aborted games.")
    | _ ->
        match game.players |> List.tryFind (fun p -> p.id = playerId) with
        | None -> Error <| HttpException(404, "Player not found.")
        | Some player ->
            match player.userId with
            | None -> Error <| HttpException(400, "Cannot remove neutral players from game.")
            | Some x ->
                if not <| (session.isAdmin
                    || game.createdByUserId = session.userId
                    || x = session.userId)
                then Error <| HttpException(403, "Cannot remove other users from game.")        
                else 
                    let effects = new ArrayList<Effect>()

                    let playerIdsToRemove =
                        match player.kind with 
                        | User -> 
                            game.players 
                            |> List.filter (fun p -> p.userId = player.userId) 
                            |> List.map (fun p -> p.id)
                        | Guest -> [playerId]
                        | _ -> List.empty //Already eliminated this case in validation above

                    effects.Add(Effect.playersRemoved(playerIdsToRemove))

                    //Cancel game if Pending and creator quit
                    if game.status = GameStatus.Pending
                        && game.createdByUserId = player.userId.Value
                        && player.kind = PlayerKind.User
                    then 
                        effects.Add(Effect.gameStatusChanged(GameStatus.Pending, GameStatus.AbortedWhilePending))
                    else ()

                    let kind = 
                        if player.userId = Some session.userId
                        then EventKind.PlayerQuit
                        else EventKind.PlayerEjected

                    Ok <| Event.create(kind, (effects |> Seq.toList))

//TOOD: Add integration tests
let fillEmptyPlayerSlots (game : Game) : Effect list AsyncHttpResult =
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
    then okTask List.empty
    else
        GameRepository.getNeutralPlayerNames()
        |> thenMap getNeutralPlayerNamesToUse
        |> thenMap (Seq.map (Effect.playerAdded << CreatePlayerRequest.neutral))    
        |> thenMap Seq.toList