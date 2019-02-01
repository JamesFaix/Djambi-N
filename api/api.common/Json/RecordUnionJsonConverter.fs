namespace Djambi.Api.Common.Json

open System
open System.IO
open FSharp.Reflection
open Newtonsoft.Json
open Djambi.Api.Common.Collections

type RecordUnionJsonConverter() =
    inherit JsonConverter()

    //Converts discriminated unions where all cases are 1 field that is a record    
    override x.CanConvert (t : Type) : bool = 
        FSharpType.IsUnion t &&
        FSharpType.GetUnionCases t 
            |> Seq.map (fun c -> c.GetFields())
            |> Seq.forall (fun fs -> 
                fs.Length = 1 && 
                FSharpType.IsRecord fs.[0].PropertyType)

    override x.WriteJson (writer : JsonWriter, 
                          value : obj, 
                          serializer : JsonSerializer) : Unit =

        let caseType = value.GetType()
        let (case, _) = FSharpValue.GetUnionFields(value, caseType)
        writer.WriteStartObject()
        writer.WritePropertyName "kind"
        writer.WriteValue case.Name

        let itemProp = caseType.GetProperty "Item"
        let recordValue = itemProp.GetValue value
        let recordType = recordValue.GetType()
        let fields = FSharpType.GetRecordFields recordType

        for p in fields do
            writer.WritePropertyName p.Name
            let propValue = p.GetValue recordValue
            serializer.Serialize(writer, propValue)

        writer.WriteEndObject()

    override x.ReadJson(reader : JsonReader, 
                        t : Type, 
                        existingValue : obj,
                        serializer : JsonSerializer) : obj =   
                        
        let tokens = ArrayList<JsonToken * obj>()
        tokens.Add (reader.TokenType, reader.Value)
        while reader.Read() do
            tokens.Add (reader.TokenType, reader.Value)

        //Find index of first token that is a property named kind
        let index = 
            tokens 
            |> Seq.findIndex (fun (t, v) -> 
                t = JsonToken.PropertyName 
                    && v.ToString() = "kind"
            )

        let (_, kind) = tokens.[index+1]

        //tokens.RemoveAt(index+1) //Remove value
        //tokens.RemoveAt(index)   //Remove name

        let json = 
            let jsonParts = 
                tokens
                |> Seq.map (fun (t, v) ->
                    match t with
                    | JsonToken.StartObject -> "{"
                    | JsonToken.EndObject -> "},"
                    | JsonToken.PropertyName -> sprintf "\"%s\":" (v.ToString())
                    | _ -> if v = null then "null" 
                           else sprintf "\"%s\"," (v.ToString())
                )

            String.Join("", jsonParts)
                .Replace(",}", "}")

        let cases = FSharpType.GetUnionCases t
        let case = cases |> Seq.find (fun c -> c.Name = kind.ToString())
        let fields = case.GetFields()
        let recordType = fields.[0].PropertyType

        use reader = new StringReader(json)
        let record = serializer.Deserialize(reader, recordType)
        let union = FSharpValue.MakeUnion(case, [|record|])

        union