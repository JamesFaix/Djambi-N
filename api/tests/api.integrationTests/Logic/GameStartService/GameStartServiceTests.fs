namespace Djambi.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.ModelExtensions
open Djambi.Api.Logic.Services
open Djambi.Api.Model

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

            let colors = startingConditions |> List.map (fun cond -> cond.colodId)
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
                Assert.Single<Piece>(grp, (fun p -> p.kind = Chief)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Diplomat)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Reporter)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Gravedigger)) |> ignore
                Assert.Single<Piece>(grp, (fun p -> p.kind = Assassin)) |> ignore
                Assert.Equal(4, grp |> List.filter (fun p -> p.kind = Thug) |> List.length)
        }

    [<Fact>]
    let ``Start game should work``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            let playerRequest : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = Some "test"
                    kind = PlayerKind.Guest
                }

            let! _ = PlayerService.addPlayerToLobby (lobby.id, playerRequest) session |> thenValue

            //Act
            let! result = GameStartService.startGame lobby.id session

            //Assert
            result |> Result.isOk |> shouldBeTrue

            let! lobbyError = LobbyRepository.getLobby lobby.id |> thenError
            Assert.Equal(404, lobbyError.statusCode)
        }

    [<Fact>]
    let ``Start game should fail if only one non-virtual player``() =
        task {
             //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            //Act
            let! result = GameStartService.startGame lobby.id session

            //Assert
            result |> shouldBeError 400 "Cannot start game with only one player."

            let! lobbyResult = LobbyRepository.getLobby lobby.id
            lobbyResult |> Result.isOk |> shouldBeTrue
        }

    [<Fact>]
    let ``Virtual players should not be in the turn cycle``() =
        task {
            //Arrange
            let! (user, session, lobby) = createUserSessionAndLobby(true) |> thenValue

            let playerRequest : CreatePlayerRequest =
                {
                    userId = Some user.id
                    name = Some "test"
                    kind = PlayerKind.Guest
                }

            let! _ = PlayerService.addPlayerToLobby (lobby.id, playerRequest) session |> thenValue

            //Act
            let! gameStartResponse = GameStartService.startGame lobby.id session |> thenValue

            //Assert
            let! players = PlayerService.getGamePlayers gameStartResponse.gameId session |> thenValue

            let virtualPlayerIds =
                players
                |> List.filter (fun p -> p.kind = PlayerKind.Virtual)
                |> List.map (fun p -> p.id)
                |> Set.ofList

            gameStartResponse.gameState.turnCycle
            |> Set.ofList
            |> Set.intersect virtualPlayerIds
            |> Set.count
            |> shouldBe 0
        }