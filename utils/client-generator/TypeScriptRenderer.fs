namespace Djambi.ClientGenerator

open System
open System.Text
open FSharp.Reflection

type TypeScriptRenderer() =
    
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

        let typeName = renderTypeName t

        sb.AppendLine(sprintf "export interface %s {" typeName) |> ignore

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
        
        let typeName = renderTypeName t
        sb.AppendLine(sprintf "export type %s =" typeName) |> ignore

        let cases = FSharpType.GetUnionCases t
        let caseTypes = 
            cases 
            |> Seq.map (fun c ->
                let field = c.GetFields().[0]
                renderTypeName field.PropertyType
            )
        let casesList = String.Join(" |\n\t", caseTypes)

        sb.AppendLine(sprintf "\t%s" casesList) |> ignore

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
        match TypeKind.fromType t with
        | TypeKind.Record -> renderRecord t
        | TypeKind.Union -> renderUnion t
        | TypeKind.Enum -> renderEnum t
        | _ -> failwith "Unsupported type"

    interface IRenderer with

        member this.renderTypes (types : Type list) : string =
            let sb = StringBuilder()

            sb.AppendLine("/*") |> ignore
            sb.AppendLine(" * This file was generated with the Client Generator utility.") |> ignore
            sb.AppendLine(" * Do not manually edit.") |> ignore
            sb.AppendLine(" */") |> ignore

            let typesWithKinds = 
                types
                |> Seq.map (fun t -> 
                    let kind = TypeKind.fromType t
                    (t, kind)
                )
                |> Seq.filter (fun (_, kind) -> kind <> TypeKind.Unsupported)
                |> Seq.sortBy (fun (t, _) -> t.Name)
                |> Seq.toList
                
            for (t, _) in typesWithKinds do
                let text = renderType t
                sb.AppendLine(text) |> ignore

            sb.ToString()