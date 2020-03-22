namespace Apex.Api.IntegrationTests.Logic.PlayerService

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Db.Interfaces

//TODO: Audit test class
type FillEmptyPlayerSlotsTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Fill empty player slots should work``() =
        //Arrange
        let session = getSessionForUser 1
        let gameRequest = getGameParameters()
        task {
            let! game = (gameMan :> IGameManager).createGame gameRequest session |> thenValue

            //Act
            let! updatedGame = TestUtilities.fillEmptyPlayerSlots game |> thenValue

            //Assert
            let! doubleCheck = (gameRepo :> IGameRepository).getGame game.id |> thenValue

            updatedGame.players.Length |> shouldBe gameRequest.regionCount
            doubleCheck |> shouldBe updatedGame

            //All players after creator are neutral
            updatedGame.players
            |> List.filter (fun p -> p.kind = PlayerKind.Neutral)
            |> List.length
            |> shouldBe (updatedGame.players.Length - 1)
        }

    [<Fact>]
    let ``Fill empty player slots should return effect for each neutral player``() =
        //Arrange
        let session = getSessionForUser 1
        let gameRequest = getGameParameters()
        task {
            let! game = (gameMan :> IGameManager).createGame gameRequest session |> thenValue

            //Act
            let! effects = playerServ.fillEmptyPlayerSlots game |> thenValue

            //Assert
            effects.Length |> shouldBe 2

            match (effects.[0], effects.[1]) with
            | (Effect.NeutralPlayerAdded p2, Effect.NeutralPlayerAdded p3) ->
                ()
            | _ -> failwith "Invalid effects."
        }