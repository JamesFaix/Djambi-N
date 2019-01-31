namespace Djambi.Api.Common.Json

open System
open FSharp.Reflection
open Newtonsoft.Json
open Djambi.Api.Common

type UnionEnumJsonConverter() =
    inherit JsonConverter()

    override x.CanConvert (t : Type) : bool =
        (TypeKind.fromType t) = TypeKind.UnionEnum

    override x.WriteJson (writer : JsonWriter, 
                          value : obj, 
                          serializer : JsonSerializer) : Unit =

        let t = value.GetType()
        let (case, _) = FSharpValue.GetUnionFields(value, t)
        let json = sprintf "\"%s\"" case.Name
        writer.WriteRawValue(json)

    override x.ReadJson(reader : JsonReader, 
                        t : Type, 
                        existingValue : obj, 
                        serializer : JsonSerializer) : obj =  
        let text = reader.Value.ToString()
        let cases = FSharpType.GetUnionCases t
        let case = cases |> Seq.find (fun c -> c.Name = text)
        FSharpValue.MakeUnion(case, [||])