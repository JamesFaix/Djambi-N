namespace Apex.Api.IntegrationTests.Logic.GameManager

open System
open FSharp.Control.Tasks
open Xunit
open Apex.Api.Common
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.IntegrationTests
open Apex.Api.Logic.Services
open Apex.Api.Model

type GetGameTests() =
    inherit TestsBase()

//TODO: Get game state should work

//TODO: Get game state should fail if not logged in

//TODO: Get game state should fail if not in game and not admin

//TODO: Get game state shoudl work if not in game, but admin
