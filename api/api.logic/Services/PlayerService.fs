module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.PlayerModel
open Djambi.Api.Model.SessionModel

let addPlayerToLobby (request : CreatePlayerRequest, session : Session) : Unit AsyncHttpResult =
    PlayerRepository.addPlayerToLobby request
    |> thenMap ignore

let removePlayerFromLobby (lobbyPlayerId : int, session : Session) : Unit AsyncHttpResult =
    PlayerRepository.removePlayerFromLobby lobbyPlayerId

let fillEmptyPlayerSlots (lobby : Lobby) (players : Player list) : Player list AsyncHttpResult =    
    let missingPlayerCount = lobby.regionCount - players.Length

    let getVirtualNamesToUse (possibleNames : string list) = 
        Enumerable.Except(
            possibleNames, 
            players |> Seq.map (fun p -> p.name), 
            StringComparer.OrdinalIgnoreCase) 
        |> Utilities.shuffle
        |> Seq.take missingPlayerCount

    if missingPlayerCount = 0
    then players |> okTask
    else
        PlayerRepository.getVirtualPlayerNames()
        |> thenMap getVirtualNamesToUse
        |> thenDoEachAsync (fun name -> 
            let request = CreatePlayerRequest.``virtual`` (lobby.id, name)
            PlayerRepository.addPlayerToLobby request
            |> thenMap ignore
        )
        |> thenBindAsync (fun _ -> PlayerRepository.getPlayers lobby.id)