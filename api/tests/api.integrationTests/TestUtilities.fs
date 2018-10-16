namespace Djambi.Tests

open System
open Microsoft.Extensions.Configuration
open Djambi.Api.Model.UserModel
open Djambi.Api.Model.LobbyModel
open Djambi.Utilities

module TestUtilities =

    let private config = 
        ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile(Environment.environmentConfigPath(6), false)
            .Build()

    let connectionString =
        config.GetConnectionString("Main")
              .Replace("{sqlAddress}", config.["sqlAddress"])

    let getCreateUserRequest() : CreateUserRequest = 
        {
            name = "Test_" + Guid.NewGuid().ToString()
            password = Guid.NewGuid().ToString()
        }

    let getCreateLobbyRequest() : CreateLobbyRequest =
        {
            regionCount = 3
            description = Some "Test"
            createdByUserId = 1
            isPublic = false
            allowGuests = false
        }