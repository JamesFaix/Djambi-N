namespace Djambi.ClientGenerator.Annotations

open System

type HttpMethod =
    | Post = 1
    | Get = 2
    | Delete = 3
    | Put = 4

type ClientSection =
    | User = 1
    | Session = 2
    | Board = 3
    | Game = 4
    | Player = 5
    | Turn = 6
    | Events = 7
    | Snapshots = 8
    | Misc = 9

[<AllowNullLiteral>] //Needed because some Reflection methods return null values for attributes that aren't found
[<AttributeUsage(
    AttributeTargets.Class ||| AttributeTargets.Enum ||| AttributeTargets.Struct, 
    AllowMultiple = false)>]
type ClientTypeAttribute(section : ClientSection) =
    inherit Attribute()
    member this.section = section

[<AllowNullLiteral>] //Needed because some Reflection methods return null values for attributes that aren't found
[<AttributeUsage(
    AttributeTargets.Method, 
    AllowMultiple = false)>]
type ClientFunctionAttribute(method : HttpMethod, route : string, section : ClientSection) =
    inherit Attribute()
    member this.method = method
    member this.route = route
    member this.section = section