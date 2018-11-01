﻿namespace Djambi.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.PlayerModel

type GameStartServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Get starting conditions should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let lobbyRequest = getCreateLobbyRequest()
            let! lobby = LobbyRepository.createLobby (lobbyRequest, session.userId) |> thenValue
            let! players = PlayerRepository.getPlayersForLobby lobby.id
                            |> thenBindAsync (PlayerService.fillEmptyPlayerSlots lobby)
                            |> thenValue

            //Act
            let startingConditions = GameStartService.getStartingConditions players

            //Assert
            Assert.Equal(lobby.regionCount, startingConditions.Length)

            let regions = startingConditions |> List.map (fun cond -> cond.region) |> List.sort
            regions |> shouldBe [0..(lobby.regionCount-1)]

            let colors = startingConditions |> List.map (fun cond -> cond.color)
            Assert.All(colors, fun c -> Assert.True(c >= 0 && c < Constants.maxRegions))
        }

    [<Fact>]
    let ``Create pieces should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let lobbyRequest = getCreateLobbyRequest()
            let! lobby = LobbyRepository.createLobby (lobbyRequest, session.userId) |> thenValue
            let! players = PlayerRepository.getPlayersForLobby lobby.id
                            |> thenBindAsync (PlayerService.fillEmptyPlayerSlots lobby)
                            |> thenValue
            let startingConditions = GameStartService.getStartingConditions players
            let board = BoardModelUtility.getBoardMetadata(lobby.regionCount)

            //Act
            let pieces = GameStartService.createPieces(board, startingConditions)

            //Assert
            Assert.Equal(lobby.regionCount * Constants.piecesPerPlayer, pieces.Length)

            let groupByPlayer = pieces |> List.groupBy (fun p -> p.originalPlayerId)
            Assert.Equal(lobby.regionCount, groupByPlayer.Length)

            for (_, grp) in groupByPlayer do
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Chief)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Diplomat)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Reporter)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Gravedigger)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.pieceType = Assassin)) |> ignore
                Assert.Equal(4, grp |> List.filter (fun p -> p.pieceType = Thug) |> List.length)
        }

    [<Fact>]
    let ``Start game should work``() =
        task {
            //Arrange
            let session = getSessionForUser 1
            let lobbyRequest = getCreateLobbyRequest()
            let! lobby = LobbyRepository.createLobby (lobbyRequest, session.userId) |> thenValue

            //Act
            let! _ = GameStartService.startGame lobby.id session |> thenValue

            //Assert
            let! lobbyError = LobbyRepository.getLobby lobby.id |> thenError
            Assert.Equal(404, lobbyError.statusCode)
        }

    [<Fact>]
    let ``Start game should fail if only one non-virtual player``() =
        task {
            failwith "Not yet implemented"
        }

    [<Fact>]
    let ``Virtual players should not be in the turn cycle``() =
        task {
            //Arrange
            //Arrange
            let session = getSessionForUser 1
            let lobbyRequest = getCreateLobbyRequest()
            let! lobby = LobbyRepository.createLobby (lobbyRequest, session.userId) |> thenValue

            //Act
            let! gameStartResponse = GameStartService.startGame lobby.id session |> thenValue

            //Assert

            let! players = PlayerService.getGamePlayers gameStartResponse.gameId session |> thenValue

            let virtualPlayerIds =
                players
                |> List.filter (fun p -> p.playerType = PlayerType.Virtual)
                |> List.map (fun p -> p.id)
                |> Set.ofList

            gameStartResponse.gameState.turnCycle
            |> Set.ofList
            |> Set.intersect virtualPlayerIds
            |> Set.count
            |> shouldBe 0
        }