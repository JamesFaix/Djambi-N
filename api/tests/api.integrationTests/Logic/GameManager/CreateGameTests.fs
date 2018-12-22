namespace Djambi.Api.IntegrationTests.Logic.GameManager

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Managers

type CreateGameTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create game should work``() =
        task {
            //Arrange
            let parameters = getGameParameters()
            let session = getSessionForUser 1

            //Act
            let! resp = GameManager.createGame parameters session
                          |> AsyncHttpResult.thenValue

            //Assert
            let game = resp.game
            game.id |> shouldNotBe 0
            game.parameters.allowGuests |> shouldBe parameters.allowGuests
            game.parameters.description |> shouldBe parameters.description
            game.parameters.isPublic |> shouldBe parameters.isPublic
            game.parameters.regionCount |> shouldBe parameters.regionCount
            game.createdByUserId |> shouldBe session.userId

            match resp.event with
            | GameCreated e ->
                e.effects.Length |> shouldBe 2
                match (e.effects.[0], e.effects.[1]) with
                | (EventEffect.GameCreated e1, EventEffect.PlayerAdded e2) ->
                    e1.value.parameters |> shouldBe parameters
                    e1.value.createdByUserId |> shouldBe session.userId

                    e2.value.userId |> shouldBe (Some session.userId)
                    e2.value.kind |> shouldBe PlayerKind.User

                | _ -> failwith "Incorrect event effects"

                ()
            | _ -> failwith "Incorrect event type"            
        }

    [<Fact>]
    let ``Create game should add self as player``() =
        task {
            //Arrange
            let parameters = getGameParameters()
            let session = getSessionForUser 1

            //Act
            let! resp = GameManager.createGame parameters session
                         |> AsyncHttpResult.thenValue

            //Assert
            let players = resp.game.players

            players.Length |> shouldBe 1
            players |> shouldExist (fun p -> p.userId = Some session.userId
                                          && p.kind = PlayerKind.User)
        }

