namespace Apex.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Db.Interfaces

type SearchRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Search games should work``() =
        //Arrange
        task {
            let userRequest = getCreateUserRequest()
            let! user = (userRepo :> IUserRepository).createUser userRequest |> thenValue

            let gameRequest = getCreateGameRequest(user.id)
            let! gameId = (gameRepo :> IGameRepository).createGame gameRequest |> thenValue
            let query = GamesQuery.empty

            //Act
            let! games = (searchRepo :> ISearchRepository).searchGames (query, user.id) |> thenValue

            //Assert
            let exists = games |> List.exists (fun l -> l.id = gameId)
            Assert.True(exists)
        }