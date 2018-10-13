[<AutoOpen>]
module Djambi.Api.ContractTests.Assertions

open System.Net
open Xunit
open Djambi.Api.Common
open Djambi.Api.WebClient

let shouldBe<'a> (expected : 'a) (actual : 'a) =
    Assert.Equal(expected, actual)

let shouldNotBe<'a> (notExpected : 'a) (actual : 'a) =
    Assert.NotEqual(notExpected, actual)

let shouldBeTrue (value : bool) =
    Assert.True(value)

let shouldBeFalse (value : bool) =
    Assert.False(value)

let shouldBeSome<'a> (o : 'a option) =
    Assert.True(o.IsSome)

let shouldBeNone<'a> (o : 'a option) =
    Assert.True(o.IsNone)

let shouldHaveStatus<'a> (statusCode : HttpStatusCode) (response : Response<'a>) =
    Assert.Equal(statusCode, response.statusCode)

let shouldBeError<'a> (statusCode : HttpStatusCode) (message : string) (response : Response<'a>) =
    Assert.Equal(statusCode, response.statusCode)
    Assert.True(response.result |> Result.isError)
    Assert.Equal(message, response.result |> Result.error)