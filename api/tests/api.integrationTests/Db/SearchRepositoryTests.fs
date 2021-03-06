﻿namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces

type SearchRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Search games should work``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let userRequest = getCreateUserRequest()
            let! user = host.Get<IUserRepository>().createUser userRequest

            let gameRequest = getCreateGameRequest(user.id)
            let! gameId = host.Get<IGameRepository>().createGame(gameRequest, true)
            let query = GamesQuery.empty

            //Act
            let! games = host.Get<ISearchRepository>().searchGames (query, user.id)

            //Assert
            let exists = games |> List.exists (fun l -> l.id = gameId)
            Assert.True(exists)
        }