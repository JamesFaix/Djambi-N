open System
open System.Text
open System.IO
open Microsoft.Extensions.Configuration
open Djambi.Utilities

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
    sideLength / (2.0 * Math.Tan(Math.PI / (float nSides)))
    
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
    (width nSides) / 2.0

let centroidYOffset nSides = 
    if isEven nSides then apothem nSides
    else radius nSides

let getPolygons () =
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

let getCsvText () = 
    let polygons = getPolygons()

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

    sb.ToString()

let getTypeScriptText() =
    let polygons = getPolygons()
    let sb = StringBuilder()

    let addLine str =
        sb.AppendLine(str) |> ignore

    let addBlankLine () = 
        addLine ""

    let addCacheArray (fieldName : string) (getValue : PolygonData -> string) : Unit =
        let values = polygons|> List.map (fun (_, data) -> getValue data)
        let values = List.append [ "undefined"; "undefined"; "undefined" ] values //Set indexes 0-2 to undefined
        addLine (sprintf "\tprivate static readonly %sCache : number[] = [" fieldName)
        for v in values do
            addLine (sprintf "\t\t%s," v)
        addLine "\t];"

    let addMethod (fieldName : string) : Unit =
        addLine (sprintf "\tpublic static %s(numberOfSides : number) : number {" fieldName)
        addLine (sprintf "\t\tif (numberOfSides < 3 || numberOfSides > 8) {")
        addLine (sprintf "\t\t\tthrow \"Unsupported number of sides: \" + numberOfSides;")
        addLine "\t\t}"
        addLine (sprintf "\t\treturn this.%sCache[numberOfSides];" fieldName)
        addLine "\t}"

    let addCachedAndMethod (fieldName : string) (getValue : PolygonData -> string) : Unit =
        addCacheArray fieldName getValue
        addBlankLine ()
        addMethod fieldName
        addBlankLine ()

    addLine("export default class PolygonData {")
    addCachedAndMethod "internalAngle" (fun d -> d.internalAngle.ToString())
    addCachedAndMethod "externalAngle" (fun d -> d.externalAngle.ToString())
    addCachedAndMethod "radius" (fun d -> d.radius.ToString())
    addCachedAndMethod "apothem" (fun d -> d.apothem.ToString())
    addCachedAndMethod "width" (fun d -> d.width.ToString())
    addCachedAndMethod "height" (fun d -> d.height.ToString())
    addCachedAndMethod "centroidXOffset" (fun d -> d.centroidXOffset.ToString())
    addCachedAndMethod "centroidYOffset" (fun d -> d.centroidYOffset.ToString())
    addLine("}")

    sb.ToString()

[<EntryPoint>]
let main _ =
    printfn "Djambi Polygon Data Generator"
    printfn "---------------------------"

    let depthFromRoot = 5
    
    let root = Environment.rootDirectory(depthFromRoot)
    let config = ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(Environment.environmentConfigPath(depthFromRoot), false)
                    .Build()

    let outputPath = Path.Combine(root, config.["OutputPath"])
    //let text = getCsvText()
    let text = getTypeScriptText()

    File.WriteAllText(outputPath, text)
    Console.WriteLine("Done")
    0 // return an integer exit code
