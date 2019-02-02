namespace Djambi.Api.Common.Json

open System
open System.IO
open FSharp.Reflection
open Newtonsoft.Json
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
        writer.WriteStartObject()
        writer.WritePropertyName "kind"
        writer.WriteValue case.Name
        writer.WritePropertyName "value"

        let itemProp = caseType.GetProperty "Item"
        let recordValue = itemProp.GetValue value

        serializer.Serialize(writer, recordValue)

        writer.WriteEndObject()

    override x.ReadJson(reader : JsonReader, 
                        t : Type, 
                        existingValue : obj,
                        serializer : JsonSerializer) : obj =   
                        
        let tokens = ArrayList<JsonToken * obj>()
        let mutable objStarts = 1
        let mutable objEnds = 0
        tokens.Add (reader.TokenType, reader.Value)

        while objStarts > objEnds && reader.Read() do
            let tt = reader.TokenType
            if tt = JsonToken.StartObject then objStarts <- objStarts + 1
            if tt = JsonToken.EndObject then objEnds <- objEnds + 1
            tokens.Add (tt, reader.Value)

        //Remove first and last tokens
        tokens.RemoveAt(0)
        tokens.RemoveAt(tokens.Count-1)

        //Find index of first token that is a property named kind
        let index = 
            tokens 
            |> Seq.findIndex (fun (t, v) -> 
                t = JsonToken.PropertyName 
                    && v.ToString() = "kind"
            )

        let (_, kind) = tokens.[index+1]

        //Remove `kind` property
        tokens.RemoveAt(index+1)
        tokens.RemoveAt(index)

        //Remove `value` property name
        let index = 
            tokens
            |> Seq.findIndex (fun (t, v) -> 
                t = JsonToken.PropertyName
                    && v.ToString() = "value"
            )

        tokens.RemoveAt(index)

        let json = 
            let jsonParts = 
                tokens
                |> Seq.map (fun (t, v) ->
                    match t with
                    | JsonToken.StartObject -> "{"
                    | JsonToken.EndObject -> "},"
                    | JsonToken.PropertyName -> sprintf "\"%s\":" (v.ToString())
                    | _ -> if v = null then "null," 
                           else sprintf "\"%s\"," (v.ToString())
                )

            String.Join("", jsonParts)
                .Replace(",}", "}")

        let json = json.Substring(0, json.Length-1) //Remove trailing comma

        let cases = FSharpType.GetUnionCases t
        let case = cases |> Seq.find (fun c -> c.Name = kind.ToString())
        let fields = case.GetFields()
        let recordType = fields.[0].PropertyType

        use reader = new StringReader(json)
        let record = serializer.Deserialize(reader, recordType)
        let union = FSharpValue.MakeUnion(case, [|record|])

        union