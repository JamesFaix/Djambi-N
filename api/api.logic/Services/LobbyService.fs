module Djambi.Api.Logic.Services.LobbyService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.SessionModel

let createLobby (request : CreateLobbyRequest, session : Session) : Lobby AsyncHttpResult =
    LobbyRepository.createLobby request

let getLobby (lobbyId : int, session : Session) : LobbyWithPlayers AsyncHttpResult =
    LobbyRepository.getLobby lobbyId
    |> thenBindAsync (fun lobby -> 
        PlayerRepository.getPlayers lobbyId
        |> thenMap (fun players -> lobby.addPlayers players)
    )

let deleteLobby (lobbyId : int, session : Session) : Unit AsyncHttpResult =
    getLobby (lobbyId, session)
    |> thenBind (fun lobby -> 
        if lobby.createdByUserId = session.userId
        then Ok lobby
        else Error <| HttpException(403, "Users can only delete lobbies that they created.")
    )
    |> thenBindAsync (fun _ -> LobbyRepository.deleteLobby lobbyId)

let getLobbies (query : LobbiesQuery, session : Session) : Lobby list AsyncHttpResult =
    LobbyRepository.getLobbies query