namespace Djambi.ClientGenerator
    
open System
open FSharp.Reflection

type TypeKind =
    | Record
    | Enum
    | Union
    | Unsupported

module TypeKind =
    let fromType (t : Type) : TypeKind =     
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
