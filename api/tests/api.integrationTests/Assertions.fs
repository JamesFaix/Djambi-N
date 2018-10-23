﻿[<AutoOpen>]
module Djambi.Api.IntegrationTests.Assertions

open Xunit
open Djambi.Api.Common

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

let shouldBeAtLeast<'a when 'a : comparison> (minimum : 'a) (actual : 'a) =
    Assert.True(actual >= minimum)

let shouldBeAtMost<'a when 'a : comparison> (maximum : 'a) (actual : 'a) =
    Assert.True(actual <= maximum)

let shouldBeError<'a> (statusCode : int) (message : string) (result : Result<'a, HttpException>) =
    Assert.True(result |> Result.isError)
    let error = result |> Result.error
    Assert.Equal(statusCode, error.statusCode)
    Assert.Equal(message, error.Message)

let shouldExist<'a> (predicate: 'a -> bool) (xs: 'a seq) =
    xs |> Seq.exists predicate
       |> shouldBeTrue