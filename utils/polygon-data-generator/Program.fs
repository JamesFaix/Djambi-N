open System
open System.Text

type Point = { x : float; y : float }

type Polygon = Point list

type PolygonData = 
    {
        internalAngle : float
        externalAngle : float
        apothem : float
        radius : float
        height : float
        width : float
        centroidXOffset : float
        centroidYOffset : float
    }

let sideLength = 1.0

let range = [3..8]

let isDivisibleBy d n =
    n % d = 0

let isEven n = 
    isDivisibleBy 2 n

let internalAngle nSides = 
    (2.0 * Math.PI) / (float nSides)

let externalAngle nSides = 
    Math.PI - (internalAngle nSides)
    
let radius nSides = 
    sideLength / (2.0 * Math.Sin(Math.PI / (float nSides)))

let apothem nSides = 
    sideLength / (2.0 * Math.Cos(Math.PI / (float nSides)))
    
let height nSides = 
    if isEven nSides then 2.0 * (apothem nSides) else (apothem nSides) + (radius nSides)

let polygon nSides = 
    let radius = radius nSides
    let internalAngle = internalAngle nSides
    [1..nSides]
    |> List.map (fun n -> 
        let fn = float n
        {
            x = radius * Math.Sin(internalAngle * fn)
            y = radius * Math.Cos(internalAngle * fn)
        }
    )

let width nSides = 
    if isDivisibleBy 4 nSides then 2.0 * (apothem nSides)
    elif isEven nSides then 2.0 * (radius nSides)
    elif nSides = 3 then sideLength
    else //Brute force
        let polygon = polygon nSides
        let ys = polygon |> List.map (fun p -> p.y)
        let maxY = ys |> Seq.max
        let minY = ys |> Seq.maxBy (fun n -> -n)
        Math.Abs(maxY - minY)

let centroidXOffset nSides = 
    let polygon = polygon nSides
    let xs = polygon |> List.map (fun p -> p.x)
    xs |> Seq.max

let centroidYOffset nSides = 
    let polygon = polygon nSides
    let ys = polygon |> List.map (fun p -> p.y)
    ys |> Seq.max

[<EntryPoint>]
let main _ =
    printfn "Djambi Polygon Data Generator"
    
    let polygons = 
        range
        |> List.map (fun nSides ->
            let data = {
                internalAngle = internalAngle nSides
                externalAngle = externalAngle nSides
                radius = radius nSides
                apothem = apothem nSides
                height = height nSides
                width = width nSides
                centroidXOffset = centroidXOffset nSides
                centroidYOffset = centroidYOffset nSides
            }
            (nSides, data)    
        )

    let headers = 
        [|
            "Sides"
            "Internal angle"
            "External angle"
            "Radius"
            "Apothem"
            "Height"
            "Width"
            "Centroid x-offset"
            "Centroid y-offset"
        |]

    let row = String.Join(',', headers);

    let sb = StringBuilder()
    sb.AppendLine(row) |> ignore

    for (nSides, data) in polygons do
        let cells = 
            [|
                nSides.ToString()
                data.internalAngle.ToString()
                data.externalAngle.ToString()
                data.radius.ToString()
                data.apothem.ToString()
                data.height.ToString()
                data.width.ToString()
                data.centroidXOffset.ToString()
                data.centroidYOffset.ToString()
            |]

        let row = String.Join(',', cells)
        sb.AppendLine(row) |> ignore

    Console.WriteLine(sb.ToString()) 

    Console.Read() |> ignore
    0 // return an integer exit code
