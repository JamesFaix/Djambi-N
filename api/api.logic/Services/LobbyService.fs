module Djambi.Api.Logic.Services.LobbyService

open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

let createLobby (request : CreateLobbyRequest) (session : Session) : GameParameters AsyncHttpResult =
    LobbyRepository.createLobby (request, session.userId)

let deleteLobby (lobbyId : int) (session : Session) : Unit AsyncHttpResult =
    if session.isAdmin
    then okTask ()
    else
        LobbyRepository.getLobby lobbyId
        |> thenBind (fun lobby ->
            if lobby.createdByUserId = session.userId
            then Ok ()
            else Error <| HttpException(403, "Cannot delete a lobby created by another user.")
        )
    |> thenBindAsync (fun _ -> LobbyRepository.deleteLobby lobbyId)

let getLobbies (query : GamesQuery) (session : Session) : GameParameters list AsyncHttpResult =
    let isViewableByActiveUser (lobby : GameParameters) : bool =
        lobby.isPublic
        || lobby.createdByUserId = session.userId

    LobbyRepository.getLobbies query
    |> thenMap (fun lobbies ->
        if session.isAdmin
        then lobbies
        else lobbies |> List.filter isViewableByActiveUser
    )

let getLobby (lobbyId : int) (session : Session) : LobbyWithPlayers AsyncHttpResult =
    LobbyRepository.getLobby lobbyId
    |> thenBindAsync (fun lobby ->
        PlayerRepository.getPlayersForLobby lobbyId
        |> thenBind (fun players ->
            if (lobby.isPublic
                || lobby.createdByUserId = session.userId
                || players |> List.exists(fun p -> p.userId.IsSome
                                                && p.userId.Value = session.userId))
            then Ok <| lobby.addPlayers players
            else Error <| HttpException(404, "Lobby not found.")
        )
    )