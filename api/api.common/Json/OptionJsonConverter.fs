﻿namespace Djambi.Api.Common.Json

open System
open FSharp.Reflection
open Newtonsoft.Json

//Taken from http://gorodinski.com/blog/2013/01/05/json-dot-net-type-converters-for-f-option-list-tuple/
type OptionJsonConverter() =
    inherit JsonConverter()
    
    override x.CanConvert (t : Type) : bool = 
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

    override x.WriteJson (writer : JsonWriter, value : obj, serializer : JsonSerializer) : Unit =
        let value = 
            if value = null then null
            else 
                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]  
        serializer.Serialize(writer, value)

    override x.ReadJson(reader : JsonReader, t : Type, existingValue : obj, serializer : JsonSerializer) : obj =        
        let innerType = t.GetGenericArguments().[0]
        let innerType = 
            if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType        
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if value = null then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])