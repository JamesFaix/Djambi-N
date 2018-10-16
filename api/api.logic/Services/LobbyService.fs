module Djambi.Api.Logic.Services.LobbyService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.SessionModel

let createLobby (request : CreateLobbyRequest, session : Session) : Lobby AsyncHttpResult =
    LobbyRepository.createLobby request

let getLobby (lobbyId : int, session : Session) : LobbyWithPlayers AsyncHttpResult =
    LobbyRepository.getLobbyWithPlayers lobbyId

let deleteLobby (lobbyId : int, session : Session) : Unit AsyncHttpResult =
    getLobby (lobbyId, session)
    |> thenBind (fun lobby -> 
        if lobby.createdByUserId = session.userId
        then Ok lobby
        else Error <| HttpException(403, "Users can only delete lobbies that they created.")
    )
    |> thenBindAsync (fun _ -> LobbyRepository.deleteLobby lobbyId)

let addPlayerToLobby (request : CreatePlayerRequest, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.addPlayerToLobby request

let removePlayerFromLobby (lobbyPlayerId : int, session : Session) : Unit AsyncHttpResult =
    LobbyRepository.removePlayerFromLobby lobbyPlayerId

let getAllLobbies (session : Session) : Lobby list AsyncHttpResult =
    LobbyRepository.getLobbies LobbiesQuery.empty
    
let getPublicLobbies (session : Session) : Lobby list AsyncHttpResult =
    let query = { LobbiesQuery.empty with isPublic = Some true }
    LobbyRepository.getLobbies query

let getUserLobbies (session : Session) : Lobby list AsyncHttpResult =
    let query = { LobbiesQuery.empty with playerUserId = Some session.userId }
    LobbyRepository.getLobbies query