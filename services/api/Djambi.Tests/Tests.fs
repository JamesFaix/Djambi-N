module Tests

open System
open Xunit
open Djambi.Model.BoardGeometry
open Djambi.Model.BoardGeometryExtensions

[<Theory>]
[<InlineData(Directions.Up, 2, RadialDirections.Clockwise, Directions.Right)>]
[<InlineData(Directions.Up, 2, RadialDirections.CounterClockwise, Directions.Left)>]
[<InlineData(Directions.Up, 8, RadialDirections.Clockwise, Directions.Up)>]
[<InlineData(Directions.Up, -8, RadialDirections.Clockwise, Directions.Up)>]
[<InlineData(Directions.Down, 2, RadialDirections.Clockwise, Directions.Left)>]
[<InlineData(Directions.Down, 2, RadialDirections.CounterClockwise, Directions.Right)>]
let ``Directions rotate returns expected output`` 
    (input : Directions, 
     amount : int, 
     radialDirection : RadialDirections,
     expectedOutput : Directions) =    
    Assert.Equal(expectedOutput, input.rotate(amount, radialDirection));
