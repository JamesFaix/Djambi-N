namespace Djambi.Api.IntegrationTests

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Repositories
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model.GameModel
open Djambi.Api.Model.SessionModel
open Djambi.Tests.TestUtilities
open System

type GameStartServiceTests() =
    do 
        SqlUtility.connectionString <- connectionString
        
    [<Fact>]
    let ``Get starting conditions should work``() =
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let! players = PlayerRepository.getPlayers lobby.id 
                            |> thenBindAsync (PlayerService.fillEmptyPlayerSlots lobby)
                            |> thenValue

            //Act
            let startingConditions = GameStartService.getStartingConditions players

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
            let! players = PlayerRepository.getPlayers lobby.id 
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
        //Arrange
        let lobbyRequest = getCreateLobbyRequest()
        task {
            let! lobby = LobbyRepository.createLobby lobbyRequest |> thenValue
            let session : Session = 
                {
                    isAdmin = false
                    userId = lobbyRequest.createdByUserId
                    token = ""
                    id = 1
                    createdOn = DateTime.UtcNow
                    expiresOn = DateTime.UtcNow
                }

            //Act
            let! _ = GameStartService.startGame lobby.id session |> thenValue

            //Assert
            let! lobbyError = LobbyRepository.getLobby(lobby.id, adminUserId) |> thenError
            Assert.Equal(404, lobbyError.statusCode)
        }