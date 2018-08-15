module Tests

open System
open Xunit
open Djambi.Model.Boards
open Djambi.Model.BoardsExtensions

[<Theory>]
[<InlineData(Directions.Up, 2, RadialDirections.Clockwise, Directions.Right)>]
[<InlineData(Directions.Up, 2, RadialDirections.CounterClockwise, Directions.Left)>]
[<InlineData(Directions.Up, 8, RadialDirections.Clockwise, Directions.Up)>]
[<InlineData(Directions.Up, -8, RadialDirections.Clockwise, Directions.Up)>]
[<InlineData(Directions.Down, 2, RadialDirections.Clockwise, Directions.Left)>]
[<InlineData(Directions.Down, 2, RadialDirections.CounterClockwise, Directions.Right)>]
[<InlineData(Directions.DownLeft, 2, RadialDirections.Clockwise, Directions.UpLeft)>]
let ``Directions rotate returns expected output`` 
    (oldDir : Directions, amount : int, 
     radialDir : RadialDirections, expDir : Directions) =    
    Assert.Equal(expDir, oldDir.rotate(amount, radialDir));

[<Theory>]
[<InlineData(0, 1, Directions.Left, Directions.Up)>]
[<InlineData(0, 1, Directions.DownLeft, Directions.UpLeft)>]
[<InlineData(0, 1, Directions.UpLeft, Directions.UpRight)>]
[<InlineData(0, 2, Directions.Down, Directions.Right)>]
[<InlineData(0, 2, Directions.DownLeft, Directions.DownRight)>]
[<InlineData(0, 2, Directions.DownRight, Directions.UpRight)>]
let ``Board adjustDirectionsForRegionBoundary returns expected output``
    (oldRegion : int, newRegion: int,
     oldDir: Directions, expDir: Directions) =
    let boardMetadata = { regionSize = 5; regionCount = 3 }
    let oldLoc = { region = oldRegion; x = 1; y = 1 }
    let newLoc = { region = newRegion; x = 1; y = 1 }
    let newDir = boardMetadata.adjustDirectionForRegionBoundary(oldLoc, newLoc, oldDir)
    Assert.Equal(expDir, newDir.Value)

[<Theory>]
[<InlineData(1,1,0, 0,1,1, Directions.Up)>]
[<InlineData(1,0,0, 1,1,1, Directions.UpRight)>]
[<InlineData(0,1,0, 1,1,2, Directions.UpRight)>]
[<InlineData(1,1,1, 0,1,2, Directions.Up)>]
[<InlineData(1,0,1, 1,1,2, Directions.UpRight)>]
[<InlineData(0,1,1, 1,1,0, Directions.UpRight)>]
[<InlineData(1,1,2, 0,1,0, Directions.Up)>]
[<InlineData(1,0,2, 1,1,0, Directions.UpRight)>]
[<InlineData(0,1,2, 1,1,1, Directions.UpRight)>]
let ``Board adjustDirectionAndNextLocationForPassingThroughCenter returns expected output``
    (oldX : int, oldY : int, oldRegion: int, 
     expX : int, expY : int, expRegion: int,
     expDir : Directions) =
     let oldLoc = { x = oldX; y = oldY; region = oldRegion }
     let expLoc = { x = expX; y = expY; region = expRegion }
     let boardMetadata = { regionSize = 5; regionCount = 3 }
     let (newDir, newLoc) = boardMetadata.adjustDirectionAndNextLocationForPassingThroughCenter(oldLoc)
     Assert.Equal(expDir, newDir)
     Assert.Equal(expLoc, newLoc)

[<Fact>]
let ``Utilities GetValues Directions returns expected output``() =
    let expected = [
            Directions.Up; 
            Directions.UpRight 
            Directions.Right
            Directions.DownRight 
            Directions.Down
            Directions.DownLeft 
            Directions.Left
            Directions.UpLeft
        ]

    let actual = Utilities.GetValues<Directions>()

    Assert.Equal<list<Directions>>(expected, actual)
