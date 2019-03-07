namespace Djambi.Api.Common.Json

open System
open Microsoft.FSharp.Reflection
open Newtonsoft.Json

//Taken from http://gorodinski.com/blog/2013/01/05/json-dot-net-type-converters-for-f-option-list-tuple/
type TupleArrayJsonConverter() =
    inherit JsonConverter()
    
    override x.CanConvert (t : Type) : bool =
        FSharpType.IsTuple(t)

    override x.WriteJson (writer : JsonWriter, value : obj, serializer : JsonSerializer) : Unit =
        let values = FSharpValue.GetTupleFields(value)
        serializer.Serialize(writer, values)

    override x.ReadJson(reader : JsonReader, t : Type, existingValue : obj, serializer : JsonSerializer) : obj =        
        let advance = reader.Read >> ignore
        let deserialize t = serializer.Deserialize(reader, t)
        let itemTypes = FSharpType.GetTupleElements(t)

        let readElements() =
            let rec read index acc =
                match reader.TokenType with
                | JsonToken.EndArray -> acc
                | _ ->
                    let value = deserialize(itemTypes.[index])
                    advance()
                    read (index + 1) (acc @ [value])
            advance()
            read 0 []

        match reader.TokenType with
        | JsonToken.StartArray ->
            let values = readElements()
            FSharpValue.MakeTuple(values |> List.toArray, t)
        | _ -> failwith "invalid token"