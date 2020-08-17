namespace Djambi.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Interfaces
open Djambi.Api.Db.Interfaces
open Djambi.Api.Enums
open Djambi.Api.Logic.Services

//TODO: Audit test class
type FillEmptyPlayerSlotsTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should work``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let! user = createUser()
            let session = getSessionForUser user
            let gameRequest = getGameParameters()
            let! game = host.Get<IGameManager>().createGame gameRequest session

            //Act
            let! updatedGame = TestUtilities.fillEmptyPlayerSlots game

            //Assert
            let! doubleCheck = host.Get<IGameRepository>().getGame game.id

            updatedGame.players.Length |> shouldBe gameRequest.regionCount
            { 
                doubleCheck with
                    createdBy = { doubleCheck.createdBy with time = updatedGame.createdBy.time } // Timestamps may vary
            }
            |> shouldBe updatedGame

            //All players after creator are neutral
            updatedGame.players
            |> List.filter (fun p -> p.kind = PlayerKind.Neutral)
            |> List.length
            |> shouldBe (updatedGame.players.Length - 1)
        }

    [<Fact>]
    let ``Fill empty player slots should return effect for each neutral player``() =
        let host = HostFactory.createHost()
        //Arrange
        task {
            let! user = createUser()
            let session = getSessionForUser user
            let gameRequest = getGameParameters()
            let! game = host.Get<IGameManager>().createGame gameRequest session

            //Act
            let! effects = host.Get<PlayerService>().fillEmptyPlayerSlots game

            //Assert
            effects.Length |> shouldBe 2

            match (effects.[0], effects.[1]) with
            | (Effect.NeutralPlayerAdded p2, Effect.NeutralPlayerAdded p3) ->
                ()
            | _ -> failwith "Invalid effects."
        }