[<AutoOpen>]
module Apex.Api.ContractTests.Assertions

open System.Net
open NUnit.Framework
open Apex.Api.Common.Control
open Apex.Api.WebClient

let shouldBe<'a> (expected : 'a) (actual : 'a) =
    Assert.AreEqual(expected, actual)

let shouldNotBe<'a> (notExpected : 'a) (actual : 'a) =
    Assert.AreNotEqual(notExpected, actual)

let shouldBeTrue (value : bool) =
    Assert.True(value)

let shouldBeFalse (value : bool) =
    Assert.False(value)

let shouldBeSome<'a> (o : 'a option) =
    Assert.True(o.IsSome)

let shouldBeNone<'a> (o : 'a option) =
    Assert.True(o.IsNone)

let shouldBeAtLeast<'a when 'a : comparison> (minimum : 'a) (actual : 'a) =
    Assert.True(actual >= minimum)

let shouldBeAtMost<'a when 'a : comparison> (maximum : 'a) (actual : 'a) =
    Assert.True(actual <= maximum)

let shouldHaveStatus<'a> (statusCode : HttpStatusCode) (response : Response<'a>) =
    Assert.AreEqual(statusCode, response.statusCode)

let shouldBeError<'a> (statusCode : HttpStatusCode) (message : string) (response : Response<'a>) =
    Assert.AreEqual(statusCode, response.statusCode)
    Assert.True(response.body |> Result.isError)
    Assert.AreEqual(message, response.body |> Result.error)

let shouldExist<'a> (predicate: 'a -> bool) (xs: 'a seq) =
    xs |> Seq.exists predicate
       |> shouldBeTrue