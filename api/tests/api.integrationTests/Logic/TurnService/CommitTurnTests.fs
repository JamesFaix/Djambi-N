namespace Djambi.Api.IntegrationTests.Logic.TurnService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model

type CommitTurnTests() =
    inherit TestsBase()

//TODO: Commit turn should work

//TODO: Commit turn should fail if not logged in

//TODO: Commit turn should fail if not current player and not admin

//TODO: Commit turn should work if not current player, but admin