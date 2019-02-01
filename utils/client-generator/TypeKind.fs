namespace Djambi.ClientGenerator
    
open System
open FSharp.Reflection

type TypeKind =
    | Record
    | UnionEnum
    | Union
    | UnionCase
    | Other

module TypeKind =
    let fromType (t : Type) : TypeKind =     
        //This is just assembly level attributes
        if t.Name = "AssemblyAttributes" then TypeKind.Other
        //Each union type contains a nested Tags type that has a constant int for each tag
        elif t.Name = "Tags" then TypeKind.Other
        //This is some noise that gets added to unions during debugging        
        elif t.Name.Contains "DebugTypeProxy" then TypeKind.Other
        //This is a module/static class
        elif t.IsAbstract && t.IsSealed then TypeKind.Other

        elif FSharpType.IsRecord t then TypeKind.Record

        elif FSharpType.IsUnion t then
            let casesAndFields =  
                FSharpType.GetUnionCases t 
                |> Seq.map (fun c -> (c, c.GetFields()))
                |> Seq.toList

            if casesAndFields |> List.forall (fun (_, fields) -> fields |> Seq.isEmpty)
            then TypeKind.UnionEnum
            elif t.IsAbstract then TypeKind.Union //This will only count the union base class
            else TypeKind.UnionCase //This allows excluding each case's derived class

        else TypeKind.Other
