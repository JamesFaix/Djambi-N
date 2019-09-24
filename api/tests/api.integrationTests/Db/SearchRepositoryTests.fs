﻿namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type SearchRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Search games should work``() =
        //Arrange
        task {
            let userRequest = getCreateUserRequest()
            let! user = db.users.createUser userRequest |> thenValue

            let gameRequest = getCreateGameRequest(user.id)
            let! gameId = db.games.createGame gameRequest |> thenValue
            let query = GamesQuery.empty

            //Act
            let! games = db.search.searchGames (query, user.id) |> thenValue

            //Assert
            let exists = games |> List.exists (fun l -> l.id = gameId)
            Assert.True(exists)
        }