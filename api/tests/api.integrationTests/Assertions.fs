[<AutoOpen>]
module Apex.Api.IntegrationTests.Assertions

open Xunit

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

let shouldBeGreaterThan<'a when 'a : comparison> (threshold : 'a) (actual : 'a) =
    Assert.True(actual > threshold)

let shouldBeLessThan<'a when 'a : comparison> (threshold : 'a) (actual : 'a) =
    Assert.True(actual < threshold)

let shouldExist<'a> (predicate : 'a -> bool) (xs : 'a seq) =
    xs |> Seq.exists predicate
       |> shouldBeTrue

let shouldNotExist<'a> (predicate : 'a -> bool) (xs : 'a seq) =
    xs |> Seq.exists predicate
       |> shouldBeFalse