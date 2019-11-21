namespace Apex.Api.IntegrationTests.Logic.TurnService

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services
open Apex.Api.Model

type CommitTurnTests() =
    inherit TestsBase()

//TODO: Commit turn should work

//TODO: Commit turn should fail if not logged in

//TODO: Commit turn should fail if not current player and not admin

//TODO: Commit turn should work if not current player, but admin