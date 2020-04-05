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
        let host = HostFactory.createHost()
        //Arrange
        task {
            let userRequest = getCreateUserRequest()
            let! user = host.Get<IUserRepository>().createUser userRequest |> thenValue

            let gameRequest = getCreateGameRequest(user.id)
            let! gameId = host.Get<IGameRepository>().createGame gameRequest |> thenValue
            let query = GamesQuery.empty

            //Act
            let! games = host.Get<ISearchRepository>().searchGames (query, user.id)

            //Assert
            let exists = games |> List.exists (fun l -> l.id = gameId)
            Assert.True(exists)
        }