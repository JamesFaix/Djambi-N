module Apex.Api.Common.Enum

open System
open FSharp.Reflection
open Apex.Api.Common.Control

let inline getValues<'a>() : 'a list =
    (Enum.GetValues(typeof<'a>) :?> ('a []))
    |> Array.toList

let parseUnion<'a> (tag : string) : 'a HttpResult=
    let t = typeof<'a>
    let kind = TypeKind.fromType t
    if kind <> TypeKind.UnionEnum
    then Error <| HttpException(500, sprintf "%s is not a enum-style union type." t.Name)
    else
        let cases = FSharpType.GetUnionCases t
        let case = cases |> Seq.tryFind (fun c -> String.Equals(c.Name, tag, StringComparison.OrdinalIgnoreCase))
        match case with
        | None -> Error <| HttpException(400, sprintf "Cannot parse '%s' as a value of type %s." tag t.Name)
        | Some c ->
            let o = FSharpValue.MakeUnion(c, [||])
            Ok (o :?> 'a)