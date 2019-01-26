namespace Djambi.ClientGenerator.Annotations

open System

[<AllowNullLiteral>] //Needed because some Reflection methods return null values for attributes that aren't found
[<AttributeUsage(
    AttributeTargets.Class ||| AttributeTargets.Enum ||| AttributeTargets.Struct, 
    AllowMultiple = false)>]
type ClientTypeAttribute() =
    inherit Attribute()

type HttpMethod =
    | Post = 1
    | Get = 2
    | Delete = 3
    | Put = 4

[<AllowNullLiteral>] //Needed because some Reflection methods return null values for attributes that aren't found
[<AttributeUsage(
    AttributeTargets.Method, 
    AllowMultiple = false)>]
type ClientFunctionAttribute(method : HttpMethod, route : string) =
    inherit Attribute()
    member this.method = method
    member this.route = route