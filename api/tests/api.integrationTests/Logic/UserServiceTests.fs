namespace Djambi.Api.IntegrationTests.Logic

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model.PlayerModel

type UserServiceTests() =
    inherit TestsBase()

   