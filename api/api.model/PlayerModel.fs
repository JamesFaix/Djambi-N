module Djambi.Api.Model.PlayerModel

open Djambi.Api.Model.Enums

type Player =
    {
        id : int
        lobbyId : int
        userId : int option
        playerType : PlayerType
        name : string
    }

type CreatePlayerRequest = 
    {
        lobbyId : int
        playerType : PlayerType
        userId : int option
        name : string option
    }

module CreatePlayerRequest =
    
    let user (lobbyId : int, userId : int) : CreatePlayerRequest =
        {
            lobbyId = lobbyId
            playerType = PlayerType.User
            userId = Some userId
            name = None
        }

    let guest (lobbyId : int, userId : int, name : string) : CreatePlayerRequest =
        {
            lobbyId = lobbyId
            playerType = PlayerType.Guest
            userId = Some userId
            name = Some name
        }

    let ``virtual`` (lobbyId : int, name : string) : CreatePlayerRequest =
        {
            lobbyId = lobbyId
            playerType = PlayerType.Virtual
            userId = None
            name = Some name
        }        