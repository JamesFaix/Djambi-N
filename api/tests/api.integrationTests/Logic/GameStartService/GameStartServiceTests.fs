namespace Apex.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Logic.ModelExtensions
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open Apex.Api.Enums

type GameStartServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get starting conditions should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let parameters = getGameParameters()
            let! game = (gameMan :> IGameManager).createGame parameters session
                        |> thenBindAsync TestUtilities.fillEmptyPlayerSlots
                        |> thenValue

            //Act
            let playersWithStartConditions = gameStartServ.assignStartingConditions game.players

            //Assert
            Assert.Equal(game.parameters.regionCount, playersWithStartConditions.Length)

            let regions = playersWithStartConditions |> List.map (fun p -> p.startingRegion.Value) |> List.sort
            regions |> shouldBe [0..(game.parameters.regionCount-1)]

            let colors = playersWithStartConditions |> List.map (fun cond -> cond.colorId.Value)
            Assert.All(colors, fun c -> Assert.True(c >= 0 && c < Constants.maxRegions))
        }

    [<Fact>]
    let ``Create pieces should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let parameters = getGameParameters()
            let! game = (gameMan :> IGameManager).createGame parameters session
                        |> thenBindAsync TestUtilities.fillEmptyPlayerSlots
                        |> thenValue
            let playersWithStartConditions = gameStartServ.assignStartingConditions game.players
            let board = BoardModelUtility.getBoardMetadata(game.parameters.regionCount)

            //Act
            let pieces = gameStartServ.createPieces(board, playersWithStartConditions)

            //Assert
            Assert.Equal(game.parameters.regionCount * Constants.piecesPerPlayer, pieces.Length)

            let groupByPlayer = pieces |> List.groupBy (fun p -> p.originalPlayerId)
            Assert.Equal(game.parameters.regionCount, groupByPlayer.Length)

            for (_, grp) in groupByPlayer do
                Assert.Single<Piece>(grp, (fun p -> p.kind = PieceKind.Conduit)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = PieceKind.Diplomat)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = PieceKind.Scientist)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = PieceKind.Reaper)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = PieceKind.Hunter)) |> ignore
                Assert.Equal(4, grp |> List.filter (fun p -> p.kind = PieceKind.Thug) |> List.length)
        }

    [<Fact>]
    let ``Neutral players should not be in the turn cycle``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let playerRequest = CreatePlayerRequest.guest (user.id, "test")

            let! _ = (gameMan :> IPlayerManager).addPlayer game.id playerRequest session |> thenValue

            //Act
            let updatedGame = gameStartServ.applyStartGame game

            //Assert
            let neutralPlayerIds =
                updatedGame.players
                |> List.filter (fun p -> p.kind = PlayerKind.Neutral)
                |> List.map (fun p -> p.id)
                |> Set.ofList

            updatedGame.turnCycle
            |> Set.ofList
            |> Set.intersect neutralPlayerIds
            |> Set.count
            |> shouldBe 0
        }