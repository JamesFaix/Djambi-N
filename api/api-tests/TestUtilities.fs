namespace Djambi.Tests

module TestUtilities =

    open System

    open Microsoft.Extensions.Configuration

    open Djambi.Utilities
    open Djambi.Api.Model.Lobby


    let private config = 
        ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile(Environment.environmentConfigPath(5), false)
            .Build()

    let connectionString =
        config.GetConnectionString("Main")
              .Replace("{sqlAddress}", config.["sqlAddress"])

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