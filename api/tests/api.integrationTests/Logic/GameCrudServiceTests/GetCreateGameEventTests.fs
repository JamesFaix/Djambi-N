namespace Djambi.Api.IntegrationTests.Logic.GameCrudService

open Xunit
open Djambi.Api.Common.Control
open Djambi.Api.IntegrationTests
open Djambi.Api.Model
open Djambi.Api.Logic.Services

type GetCreateGameEventTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Should work``() =
        //Arrange
        let parameters = getGameParameters()
        let session = getSessionForUser 1

        //Act
        let event = GameCrudService.getCreateGameEvent parameters session |> Result.value

        //Assert
        event.kind |> shouldBe EventKind.GameCreated
        event.effects.Length |> shouldBe 2
        event.effects |> shouldExist (fun f -> match f with | Effect.GameCreated _ -> true | _ -> false)
        event.effects |> shouldExist (fun f -> match f with | Effect.PlayerAdded _ -> true | _ -> false)