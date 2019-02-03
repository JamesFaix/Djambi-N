namespace Djambi.Api.IntegrationTests.Logic.GameStartService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.Api.Db.Repositories

type GetGameStartEventTests() =
    inherit TestsBase()
    
    let createUserSessionAndGameWith3Players() =
        task {
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue
            
            let player2request = 
                { TestUtilities.getCreatePlayerRequest with
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }
            let! _ = GameRepository.addPlayer (game.id, player2request) |> thenValue
            
            let player3request = { player2request with name = Some "p3" }
            let! _ = GameRepository.addPlayer (game.id, player3request) |> thenValue

            let! game = GameRepository.getGame game.id |> thenValue

            return Ok (user, session, game)
        }

    [<Fact>]
    let ``Should fail not admin or creator``() =
        task {
            //Arrange            
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)

            //Act
            let! result = GameStartService.getGameStartEvent game session

            //Assert
            result |> shouldBeError 403 SecurityService.notAdminOrCreatorErrorMessage
        }
        
    [<Fact>]
    let ``Should fail if only one player``() =
        task {
            //Arrange            
            let! (_, session, game) = createuserSessionAndGame(true) |> thenValue

            //Act
            let! result = GameStartService.getGameStartEvent game session

            //Assert
            result |> shouldBeError 400 "Cannot start game with only one player."
        }

    [<Fact>]
    let ``Should work if admin``() =
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue
            let session = session |> TestUtilities.setSessionUserId (session.user.id+1)
                                  |> TestUtilities.setSessionIsAdmin true

            //Act
            let! event = GameStartService.getGameStartEvent game session |> thenValue

            //Assert
            event.kind |> shouldBe EventKind.GameStarted
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with
            | Effect.GameStatusChanged f ->
                f.oldValue |> shouldBe GameStatus.Pending
                f.newValue |> shouldBe GameStatus.Started

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should work if creator``() =
        task {
            //Arrange
            let! (user, session, game) = createUserSessionAndGameWith3Players() |> thenValue

            //Act
            let! event = GameStartService.getGameStartEvent game session |> thenValue

            //Assert
            event.kind |> shouldBe EventKind.GameStarted
            event.effects.Length |> shouldBe 1
            match event.effects.[0] with
            | Effect.GameStatusChanged f ->
                f.oldValue |> shouldBe GameStatus.Pending
                f.newValue |> shouldBe GameStatus.Started

            | _ -> failwith "Incorrect effects"
        }
    
    [<Fact>]
    let ``Should add players if not at capacity``() =
        task {
            //Arrange
            let! (user, session, game) = createuserSessionAndGame(true) |> thenValue

            let p2Request =
                {
                    userId = Some user.id
                    kind = PlayerKind.Guest
                    name = Some "p2"
                }

            let! _ = GameRepository.addPlayer(game.id, p2Request)
            let! game = GameRepository.getGame game.id |> thenValue

            //Act
            let! event = GameStartService.getGameStartEvent game session |> thenValue

            //Assert
            event.kind |> shouldBe EventKind.GameStarted
            event.effects.Length |> shouldBe 2
            match (event.effects.[0], event.effects.[1]) with
            | (Effect.PlayerAdded f1, Effect.GameStatusChanged f2) ->
                f1.playerRequest.kind |> shouldBe PlayerKind.Neutral
                f1.playerRequest.userId |> shouldBeNone

                f2.oldValue |> shouldBe GameStatus.Pending
                f2.newValue |> shouldBe GameStatus.Started
                
            | _ -> failwith "Incorrect effects"
        }