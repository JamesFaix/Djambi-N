namespace Djambi.Api.Http

module LobbyMappings =

    open Djambi.Api.Domain.LobbyModels
    open Djambi.Api.Http.LobbyJsonModels

    let mapUserResponse(user : User) : UserJsonModel =
        {
            id = user.id
            name = user.name
        }

    let mapCreateUserRequest(request : CreateUserJsonModel) : CreateUserRequest =
        {
            name = request.name
        }

    let mapLobbyGameResponse(game : LobbyGameMetadata) : LobbyGameJsonModel =
        {
            id = game.id
            status = game.status
            boardRegionCount = game.boardRegionCount
            description = game.description
            players = game.players |> List.map mapUserResponse
        }

    let mapCreateGameRequest(request : CreateGameJsonModel) : CreateGameRequest =
        {
            boardRegionCount = request.boardRegionCount
            description = if request.description = null then None else Some request.description
        }