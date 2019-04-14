namespace Djambi.Api.Common.Json

open System
open System.IO
open FSharp.Reflection
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Djambi.Api.Common.Collections

type SingleFieldUnionJsonConverter() =
    inherit JsonConverter()

    override x.CanConvert (t : Type) : bool =
        FSharpType.IsUnion t &&
        FSharpType.GetUnionCases t
            |> Seq.forall (fun c -> c.GetFields().Length = 1)

    override x.WriteJson (writer : JsonWriter,
                          value : obj,
                          serializer : JsonSerializer) : Unit =

        let caseType = value.GetType()
        let (case, _) = FSharpValue.GetUnionFields(value, caseType)
        let itemProp = caseType.GetProperty "Item"
        let recordValue = itemProp.GetValue value

        writer.WriteStartObject()
        writer.WritePropertyName "kind"
        writer.WriteValue case.Name
        writer.WritePropertyName "value"
        serializer.Serialize(writer, recordValue)
        writer.WriteEndObject()

    override x.ReadJson(reader : JsonReader,
                        t : Type,
                        existingValue : obj,
                        serializer : JsonSerializer) : obj =
        let jobj = JObject.Load(reader)
        let kind = jobj.["kind"].ToString()

        let cases = FSharpType.GetUnionCases t
        let case = cases |> Seq.find (fun c -> c.Name = kind)
        let recordType = case.GetFields().[0].PropertyType

        use reader = new StringReader(jobj.["value"].ToString())
        let record = serializer.Deserialize(reader, recordType)
        let union = FSharpValue.MakeUnion(case, [|record|])

        union