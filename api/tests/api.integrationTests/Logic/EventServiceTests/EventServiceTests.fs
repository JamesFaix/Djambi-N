namespace Djambi.Api.IntegrationTests.Logic.EventServiceTests

open Xunit
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type EventServiceTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should apply effects in order``() =
        //Arrange
        let game = TestUtilities.defaultGame

        let effects = 
            [
                //If the status Started is used here additional setup is required to make `game` valid
                Effect.gameStatusChanged(GameStatus.Pending, GameStatus.AbortedWhilePending)
                Effect.gameStatusChanged(GameStatus.AbortedWhilePending, GameStatus.Aborted)
            ]

        let event = Event.create(EventKind.GameStarted, effects) //Doesn't matter what kind
        
        //Act
        let newGame = EventService.applyEvent game event

        //Assert
        newGame.status |> shouldBe GameStatus.Aborted