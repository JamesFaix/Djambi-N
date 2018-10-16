namespace Djambi.Api.IntegrationTests

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model.LobbyModel
open Djambi.Api.Model.PlayModel
open Djambi.Tests.TestUtilities

type GameStartServiceTests() =
    do 
        SqlUtility.connectionString <- connectionString

    [<Fact>]
    let ``Add virtual players should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! lobbyWithPlayers = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue

            //Act
            let! lobbyWithVirtualPlayers = GameStartService.addVirtualPlayers lobbyWithPlayers |> thenValue

            //Assert
            let! updatedPlayers = LobbyRepository.getLobbyPlayers lobby.id |> thenValue
            Assert.Equal(lobbyRequest.regionCount, lobbyWithVirtualPlayers.players.Length)
            Assert.Equal<LobbyPlayer list>(updatedPlayers, lobbyWithVirtualPlayers.players)
        }

    [<Fact>]
    let ``Get starting conditions should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! lobbyWithPlayers = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue
            let! lobbyWithVirtualPlayers = GameStartService.addVirtualPlayers lobbyWithPlayers |> thenValue

            //Act
            let startingConditions = GameStartService.getStartingConditions lobbyWithVirtualPlayers.players

            //Assert
            Assert.Equal(lobby.regionCount, startingConditions.Length)

            let turnNumbers = startingConditions |> List.map (fun cond -> cond.turnNumber) |> List.sort
            Assert.Equal<int list>([0..(lobby.regionCount-1)], turnNumbers)

            let regions = startingConditions |> List.map (fun cond -> cond.region) |> List.sort
            Assert.Equal<int list>([0..(lobby.regionCount-1)], regions)

            let colors = startingConditions |> List.map (fun cond -> cond.color)
            Assert.All(colors, fun c -> Assert.True(c >= 0 && c < Constants.maxRegions))
        }

    [<Fact>]
    let ``Create pieces should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! lobbyWithPlayers = LobbyRepository.getLobbyWithPlayers lobby.id |> thenValue
            let! lobbyWithVirtualPlayers = GameStartService.addVirtualPlayers lobbyWithPlayers |> thenValue
            let startingConditions = GameStartService.getStartingConditions lobbyWithVirtualPlayers.players
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
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue

            //Act
            let! _ = GameStartService.startGame lobby.id |> thenValue

            //Assert
            let! updated = LobbyRepository.getLobby lobby.id |> thenValue

            //Assert.Equal(GameStatus.Started, updated.status)
            ()
        }