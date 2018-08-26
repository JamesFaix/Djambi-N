namespace Djambi.Tests

open System
open Djambi.Api.Domain.LobbyModels

module TestUtilities =

    let connectionString =
        "Data Source=localhost;Initial Catalog=Djambi;Persist Security Info=True;Integrated Security=True;MultiSubnetFailover=True;Application Name=Djambi.API"

    let getCreateUserRequest() : CreateUserRequest = 
        {
            name = "Test_" + Guid.NewGuid().ToString()
            role = Normal
            password = Guid.NewGuid().ToString()
        }

    let getCreateGameRequest() : CreateGameRequest =
        {
            boardRegionCount = 3
            description = Some "Test"
        }
