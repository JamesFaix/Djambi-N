namespace Djambi.Api.Http

module LobbyJsonMappings =

    open System
    open Djambi.Api.Model.Lobby
    open Djambi.Api.Http.LobbyJsonModels

    let mapRoleFromString(roleName : string) : Role =
        match roleName.ToUpperInvariant() with
        | "ADMIN" -> Admin
        | "NORMAL" -> Normal
        | "GUEST" -> Guest
        | _ -> failwith ("Invalid role name: " + roleName)

    let mapUserResponse(user : User) : UserJsonModel =
        {
            id = user.id
            name = user.name
            role = user.role.ToString()
        }

    let mapPlayerResponse(player : LobbyPlayer) : PlayerJsonModel =
        {
            id = player.id
            userId = if player.userId.IsSome 
                     then new Nullable<int>(player.userId.Value) 
                     else Unchecked.defaultof<int Nullable>
            name = player.name
        }

    let mapCreateUserRequest(jsonModel : CreateUserJsonModel) : CreateUserRequest =
        {
            name = jsonModel.name
            role = jsonModel.role |> mapRoleFromString
            password = jsonModel.password
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

    let mapCreateGameRequest(jsonModel : CreateGameJsonModel) : CreateGameRequest =
        {
            boardRegionCount = jsonModel.boardRegionCount
            description = if jsonModel.description = null then None else Some jsonModel.description
        }

    let mapLoginRequestFromJson(jsonModel : LoginRequestJsonModel) : LoginRequest =
        {
            userName = jsonModel.userName
            password = jsonModel.password
        }