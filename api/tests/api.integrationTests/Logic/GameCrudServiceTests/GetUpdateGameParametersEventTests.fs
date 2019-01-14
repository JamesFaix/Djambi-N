namespace Djambi.Api.IntegrationTests.Logic.GameCrudService

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services
open Djambi.Api.Db.Repositories

type GetUpdateGameParametersEventTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should work``() =
        task {
            //Arrange
            let! (_, session, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let newParameters = 
                {
                    allowGuests = false
                    isPublic = true
                    description = Some "new description"
                    regionCount = 5
                }

            //Act
            let event = GameCrudService.getUpdateGameParametersEvent (game, newParameters) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.GameParametersChanged
            event.effects.Length |> shouldBe 1

            match event.effects.[0] with
            | Effect.ParametersChanged f ->
                f.oldValue |> shouldBe game.parameters
                f.newValue |> shouldBe newParameters

            | _ -> failwith "Incorrect effects"
        }

    
    [<Fact>]
    let ``Should have effect for truncating extra players if region count being lowered``() =
        task {
            //Arrange
            let! (user, session, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let! _ = GameRepository.updateGameParameters game.id { game.parameters with regionCount = 4 } |> thenValue

            for n in [2..4] do
                let playerRequest = 
                    { TestUtilities.getCreatePlayerRequest with 
                        userId = Some user.id;
                        kind = PlayerKind.Guest; 
                        name = Some (sprintf "p%i" n)
                    }
                let! _ = GameRepository.addPlayer (game.id, playerRequest)
                ()

            let! game = GameRepository.getGame game.id |> thenValue

            let newParameters = { game.parameters with regionCount = 3}

            //Act
            let event = GameCrudService.getUpdateGameParametersEvent (game, newParameters) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.GameParametersChanged
            event.effects.Length |> shouldBe 2

            match (event.effects.[0], event.effects.[1]) with
            | (Effect.ParametersChanged f1, Effect.PlayersRemoved f2) ->
                f1.oldValue |> shouldBe game.parameters
                f1.newValue |> shouldBe newParameters
                f2.value |> shouldBe [game.players.[3].id]

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should have effect for remvoving guests if allowGuests is being disabled``() =
        task {
            //Arrange
            let! (user, session, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            for n in [2..3] do
                let playerRequest = 
                    { TestUtilities.getCreatePlayerRequest with 
                        userId = Some user.id;
                        kind = PlayerKind.Guest; 
                        name = Some (sprintf "p%i" n)
                    }
                let! _ = GameRepository.addPlayer (game.id, playerRequest)
                ()

            let! game = GameRepository.getGame game.id |> thenValue

            let newParameters = { game.parameters with allowGuests = false }

            //Act
            let event = GameCrudService.getUpdateGameParametersEvent (game, newParameters) session |> Result.value

            //Assert
            event.kind |> shouldBe EventKind.GameParametersChanged
            event.effects.Length |> shouldBe 2

            match (event.effects.[0], event.effects.[1]) with
            | (Effect.ParametersChanged f1, Effect.PlayersRemoved f2) ->
                f1.oldValue |> shouldBe game.parameters
                f1.newValue |> shouldBe newParameters
                f2.value |> shouldBe [game.players.[1].id; game.players.[2].id]

            | _ -> failwith "Incorrect effects"
        }

    [<Fact>]
    let ``Should fail if game not pending``() =
        task {
            //Arrange
            let! (_, session, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let request : UpdateGameStateRequest =
                {
                    gameId = game.id
                    status = GameStatus.Started
                    pieces = game.pieces
                    currentTurn = game.currentTurn
                    turnCycle = game.turnCycle
                }

            let! _ = GameRepository.updateGameState request |> thenValue
            let! game = GameRepository.getGame game.id |> thenValue

            let newParameters = game.parameters

            //Act
            let result = GameCrudService.getUpdateGameParametersEvent (game, newParameters) session

            //Assert
            result |> shouldBeError 400 "Cannot change game parameters unless game is Pending."
        }

    [<Fact>]
    let ``Should fail if not admin or game creator``() =
        task {
            //Arrange
            let! (_, session, game) = TestUtilities.createuserSessionAndGame(true) |> thenValue

            let newParameters = game.parameters

            //Act
            let result = GameCrudService.getUpdateGameParametersEvent (game, newParameters) { session with userId = session.userId + 1 }

            //Assert
            result |> shouldBeError 403 "Cannot change game parameters of game created by another user."
        }