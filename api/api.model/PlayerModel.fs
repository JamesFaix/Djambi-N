[<AutoOpen>]
module Djambi.Api.Model.PlayerModel

type PlayerKind =
    | User
    | Guest
    | Virtual

type Player =
    {
        id : int
        lobbyId : int
        userId : int option
        kind : PlayerKind
        name : string
    }

[<CLIMutable>]
type CreatePlayerRequest = 
    {
        kind : PlayerKind
        userId : int option
        name : string option
    }

module CreatePlayerRequest =
    
    let user (userId : int) : CreatePlayerRequest =
        {
            kind = PlayerKind.User
            userId = Some userId
            name = None
        }

    let guest (userId : int, name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Guest
            userId = Some userId
            name = Some name
        }

    let ``virtual`` (name : string) : CreatePlayerRequest =
        {
            kind = PlayerKind.Virtual
            userId = None
            name = Some name
        }        