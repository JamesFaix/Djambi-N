module Djambi.Api.Web.Managers.PlayerManager

open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Common

let addPlayerToLobby (request : CreatePlayerRequest, lobbyId : int) (session : Session) : Player AsyncHttpResult =
    PlayerService.addPlayerToLobby (lobbyId, request) session

let removePlayerFromLobby (lobbyId : int, playerId : int) (session : Session) : Unit AsyncHttpResult =
    PlayerService.removePlayerFromLobby (lobbyId, playerId) session