namespace Djambi.Api.Http

module LobbyJsonMappings =

    open System
    open Djambi.Api.Domain.LobbyModels
    open Djambi.Api.Http.LobbyJsonModels

    let mapUserResponse(user : User) : UserJsonModel =
        {
            id = user.id
            name = user.name
            isGuest = user.isGuest
            isAdmin = user.isAdmin
        }

    let mapPlayerResponse(player : LobbyPlayer) : PlayerJsonModel =
        {
            id = player.id
            userId = if player.userId.IsSome 
                     then new Nullable<int>(player.userId.Value) 
                     else Unchecked.defaultof<int Nullable>
            name = player.name
        }

    let mapCreateUserRequest(request : CreateUserJsonModel) : CreateUserRequest =
        {
            name = request.name
            isGuest = request.isGuest
        }

    let mapLobbyGameResponse(game : LobbyGameMetadata) : LobbyGameJsonModel =
        {
            id = game.id
            status = game.status.ToString()
            boardRegionCount = game.boardRegionCount
            description = if game.description.IsSome 
                          then game.description.Value 
                          else Unchecked.defaultof<string>
            players = game.players |> List.map mapPlayerResponse
        }

    let mapCreateGameRequest(request : CreateGameJsonModel) : CreateGameRequest =
        {
            boardRegionCount = request.boardRegionCount
            description = if request.description = null then None else Some request.description
        }