namespace Djambi.ClientGenerator

open System
open System.Reflection
open System.Text
open System.Text.RegularExpressions
open FSharp.Reflection
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations

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
            typeof<Unit>, "{}"
        ]

    let rec renderTypeName (t : Type, forDeclaration : bool) : string = 
        if t.IsGenericType then    
            let name = t.GetGenericTypeDefinition().Name
            let args = t.GetGenericArguments()

            match (name, args.Length) with
            | ("FSharpList`1", 1) ->
                sprintf "%s[]" (renderTypeName (args.[0], forDeclaration))
            | ("FSharpOption`1", 1) ->
                renderTypeName(args.[0], forDeclaration)
            | _ ->             
                let nameLength = t.Name.IndexOf('`')
                let genericName = t.Name.Substring(0, nameLength)
                let argNames = args |> Seq.map (fun a -> renderTypeName(a, forDeclaration))
                let argsList = String.Join(", ", argNames)
                sprintf "%s<%s>" genericName argsList
        else
            match typeNameMapping |> List.tryFind (fun (t1, _) -> t = t1) with
            | Some (_, n) -> n
            | _ -> 
                if forDeclaration
                then t.Name
                else "Model." + t.Name

    let renderRecordDeclaration (t : Type) : string =
        let sb = StringBuilder()

        let typeName = renderTypeName(t, true)

        sb.AppendLine(sprintf "export interface %s {" typeName) |> ignore

        let props = t.GetProperties()
        for p in props do
            //Formats property like
            //Name : Type,
            let typeName = renderTypeName(p.PropertyType, true)
            sb.AppendLine(sprintf "\t%s : %s," p.Name typeName) |> ignore
            ()

        sb.AppendLine("}")
          .ToString()

    let renderUnionDeclaration (t : Type) : string =
        let sb = StringBuilder()
        
        let typeName = renderTypeName(t, true)
        sb.AppendLine(sprintf "export type %s =" typeName) |> ignore

        let cases = FSharpType.GetUnionCases t
        let caseTypes = 
            cases 
            |> Seq.map (fun c ->
                let field = c.GetFields().[0]
                renderTypeName(field.PropertyType, true)
            )
        let casesList = String.Join(" |\n\t", caseTypes)

        sb.AppendLine(sprintf "\t%s" casesList)
          .ToString()

    let renderEnumDeclaration (t : Type) : string =
        let sb = StringBuilder()
        sb.AppendLine(sprintf "export enum %s {" t.Name) |> ignore

        let tagsType = t.GetNestedTypes() |> Seq.head
        let fields = tagsType.GetFields()

        for f in fields do
            //Formats enum value like 
            //Foo = "Foo",
            sb.AppendLine(sprintf "\t%s = \"%s\"," f.Name f.Name) |> ignore
            ()

        sb.AppendLine("}")
          .ToString()
        
    let renderTypeDeclaration (t : Type) : string =
        match TypeKind.fromType t with
        | TypeKind.Record -> renderRecordDeclaration t
        | TypeKind.Union -> renderUnionDeclaration t
        | TypeKind.Enum -> renderEnumDeclaration t
        | _ -> failwith "Unsupported type"

    let renderMethod (m : MethodInfo) : string = 
        let attribute = m.GetCustomAttribute<ClientFunctionAttribute>()
        
        let rec unboxType (t : Type) : Type =
            match t.Name with
            | "Task`1"
            | "FSharpResult`2" -> unboxType (t.GetGenericArguments().[0])
            | _ -> t

        let returnType = unboxType m.ReturnType
        
        let parameters = 
            m.GetParameters()
            |> Seq.map (fun p -> (p.Name, p.ParameterType))
            //Filter out session parameters because they are generated based on authentication
            //Other parameters must come from the URL or request body
            |> Seq.filter (fun (_, t) -> t <> typeof<Session> && t <> typeof<Session option>)
            |> Seq.toList

        let routeSections = Regex.Split(attribute.route, "%\w")
        let routeParams = parameters |> List.take (routeSections.Length-1)
        let bodyParam =
            if routeParams.Length = parameters.Length 
            then None
            else Some parameters.[parameters.Length-1]

        let bodyParamType = 
            match bodyParam with
            | Some (n, t) -> t
            | _ -> typeof<Unit>

        let paramList = 
            let xs = 
                parameters 
                |> List.map (fun (n, t) -> 
                    let typeName = renderTypeName(t, false)
                    sprintf "%s : %s" n typeName
                )
            String.Join(", ", xs)

        let returnTypeName = renderTypeName(returnType, false)
        let bodyTypeName = renderTypeName(bodyParamType, false)

        let sb = StringBuilder()
        //Function declaration
        sb.AppendLine(sprintf "\tasync %s(%s) : Promise<%s> {" m.Name paramList returnTypeName) |> ignore
        
        //Add route concatentation
        sb.Append(sprintf "\t\tconst route = \"%s\"" routeSections.[0]) |> ignore

        for n in [0..routeParams.Length-1] do
            let (paramName, _) = routeParams.[n]
            sb.Append(sprintf " + %s + \"%s\"" paramName routeSections.[n+1]) |> ignore

        sb.AppendLine(";") |> ignore

        //Add ApiClientCore call
        sb.AppendLine(sprintf "\t\treturn await ApiClientCore.sendRequest<%s, %s>(" bodyTypeName returnTypeName)
          .Append(sprintf "\t\t\tHttpMethod.%s, route" (attribute.method.ToString())) |> ignore
        match bodyParam with
        | Some (n, _) -> sb.Append(sprintf ", %s" n) |> ignore
        | _ -> ()
        sb.AppendLine(");") |> ignore

        //Close function
        sb.AppendLine("\t}") |> ignore
        sb.ToString()

    let addWarningHeader (sb : StringBuilder) : Unit =
        sb.AppendLine("/*") 
          .AppendLine(" * This file was generated with the Client Generator utility.") 
          .AppendLine(" * Do not manually edit.")
          .AppendLine(" */") |> ignore

    interface IRenderer with
    
        member this.name
            with get() = "TypeScript"

        member this.modelOutputPathSetting 
            with get() = "TypeScriptModelOutputPath"

        member this.endpointsOutputPathSetting 
            with get() = "TypeScriptEndpointsOutputPath"

        member this.renderModel (types : Type list) : string =
            let sb = StringBuilder()
            addWarningHeader sb

            sb.AppendLine() |> ignore

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
                let text = renderTypeDeclaration t
                sb.AppendLine(text) |> ignore

            sb.ToString()

        member this.renderFunctions (methods : MethodInfo list) : string =
            let sb = StringBuilder()
            addWarningHeader sb

            sb.AppendLine()
              .AppendLine("import * as Model from './model';")
              .AppendLine("import {ApiClientCore, HttpMethod} from './clientCore';")
              .AppendLine()
              .AppendLine("export default class ApiClient {") 
              .AppendLine()
              |> ignore

            let methods = methods |> Seq.sortBy (fun m -> m.Name)

            for m in methods do
                let text = renderMethod m
                sb.AppendLine(text) |> ignore

            sb.AppendLine("}")
              .ToString()