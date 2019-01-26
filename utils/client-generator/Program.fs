open System
open System.IO
open System.Reflection
open System.Text
open FSharp.Reflection
open Microsoft.Extensions.Configuration
open Djambi.Utilities

type TypeKind =
    | Record
    | Enum
    | Union
    | Unsupported

let getTypeKind (t : Type) : TypeKind =
    
    //This is just assembly level attributes
    if t.Name = "AssemblyAttributes" then TypeKind.Unsupported
    //Each union type contains a nested Tags type that has a constant int for each tag
    elif t.Name = "Tags" then TypeKind.Unsupported
    //This is some noise that gets added to unions during debugging        
    elif t.Name.Contains "DebugTypeProxy" then TypeKind.Unsupported
    //This is a module/static class
    elif t.IsAbstract && t.IsSealed then TypeKind.Unsupported

    elif FSharpType.IsRecord t then TypeKind.Record

    elif FSharpType.IsUnion t then
        let casesAndFields =  
            FSharpType.GetUnionCases t 
            |> Seq.map (fun c -> (c, c.GetFields()))
            |> Seq.toList

        if casesAndFields |> List.forall (fun (_, fields) -> fields |> Seq.isEmpty)
        then TypeKind.Enum
        elif t.IsAbstract then TypeKind.Union //This will only count the union base class
        else TypeKind.Unsupported //This excludes each case's derived class

    else TypeKind.Unsupported

let typeNameMapping = 
    [
        typeof<string>, "string"
        typeof<bool>, "boolean"
        typeof<DateTime>, "Date"
        typeof<obj>, "any"
        typeof<byte>, "number"
        typeof<sbyte>, "number"
        typeof<uint16>, "number"
        typeof<int16>, "number"
        typeof<uint32>, "number"
        typeof<int32>, "number"
        typeof<uint64>, "number"
        typeof<int64>, "number"
        typeof<single>, "number"
        typeof<double>, "number"
    ]

let rec renderTypeName (t : Type) : string = 
    if t.IsGenericType then    
        let name = t.GetGenericTypeDefinition().Name
        let args = t.GetGenericArguments()

        match (name, args.Length) with
        | ("FSharpList`1", 1) ->
            sprintf "%s[]" (renderTypeName args.[0])
        | ("FSharpOption`1", 1) ->
            renderTypeName args.[0]
        | _ ->             
            let nameLength = t.Name.IndexOf('`')
            let genericName = t.Name.Substring(0, nameLength)
            let argNames = args |> Seq.map renderTypeName 
            let argsList = String.Join(", ", argNames)
            sprintf "%s<%s>" genericName argsList
    else
        match typeNameMapping |> List.tryFind (fun (t1, _) -> t = t1) with
        | Some (_, n) -> n
        | _ -> t.Name

let renderRecord (t : Type) : string =
    let sb = StringBuilder()

    sb.AppendLine(sprintf "export interface %s {" t.Name) |> ignore

    let props = t.GetProperties()
    for p in props do
        //Formats property like
        //Name : Type,
        let typeName = renderTypeName p.PropertyType
        sb.AppendLine(sprintf "\t%s : %s," p.Name typeName) |> ignore
        ()

    sb.AppendLine("}") |> ignore

    sb.ToString()

let renderUnion (t : Type) : string =
    let sb = StringBuilder()
    sb.AppendLine(sprintf "// %s - Union types not yet supported" t.Name) |> ignore
    sb.ToString()

let renderEnum (t : Type) : string =
    let sb = StringBuilder()

    sb.AppendLine(sprintf "export enum %s {" t.Name) |> ignore

    let tagsType = t.GetNestedTypes() |> Seq.head
    let fields = tagsType.GetFields()

    for f in fields do
        //Formats enum value like 
        //Foo = "Foo",
        sb.AppendLine(sprintf "\t%s = \"%s\"," f.Name f.Name) |> ignore
        ()

    sb.AppendLine("}") |> ignore

    sb.ToString()

let renderType (t : Type) : string =
    match getTypeKind t with
    | TypeKind.Record -> renderRecord t
    | TypeKind.Union -> renderUnion t
    | TypeKind.Enum -> renderEnum t
    | _ -> failwith "Unsupported type"

[<EntryPoint>]
let main argv =    
   
    let depthFromRoot = 5
    
    let root = Environment.rootDirectory(depthFromRoot)
    let config = ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(Environment.environmentConfigPath(depthFromRoot), false)
                    .Build()
                    
    let modelAssemblyPath = Path.Combine(root, config.["ModelAssemblyPath"])
    let modelOutputPath = Path.Combine(root, config.["ModelOutputPath"])
    
    let assembly = Assembly.LoadFile modelAssemblyPath
    
    let typesAndKinds = 
        assembly.GetTypes()
        |> Seq.map(fun t -> 
            let kind = getTypeKind t
            (t, kind)
        )
        |> Seq.filter (fun (_, kind) -> kind <> TypeKind.Unsupported)

    let sb = StringBuilder()

    for (t, _) in typesAndKinds do
        let text = renderType t
        sb.AppendLine text |> ignore
    
    let fullText = sb.ToString()

    File.WriteAllText(modelOutputPath, fullText)
    Console.Write fullText

    Console.Read() |> ignore

    0 // return an integer exit code