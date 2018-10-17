module Djambi.Api.Logic.Services.LobbyService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.SessionModel

let createLobby (request : CreateLobbyRequest) (session : Session) : Lobby AsyncHttpResult =
    LobbyRepository.createLobby request

let deleteLobby (lobbyId : int) (session : Session) : Unit AsyncHttpResult =
    if session.isAdmin
    then okTask ()
    else 
        LobbyRepository.getLobby lobbyId
        |> thenBind (fun lobby -> 
            if lobby.createdByUserId = session.userId
            then Ok ()
            else Error <| HttpException(403, "Users can only delete lobbies that they created.")
        )
    |> thenBindAsync (fun _ -> LobbyRepository.deleteLobby lobbyId)

let getLobbies (query : LobbiesQuery) (session : Session) : Lobby list AsyncHttpResult =
    let updatedQuery = 
        if session.isAdmin
        then query
        else { query with callingUserId = Some session.userId }
    LobbyRepository.getLobbies updatedQuery