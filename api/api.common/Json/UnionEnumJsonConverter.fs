namespace Apex.Api.Common.Json

open System
open FSharp.Reflection
open Newtonsoft.Json

type UnionEnumJsonConverter() =
    inherit JsonConverter()

    override x.CanConvert (t : Type) : bool =
        FSharpType.IsUnion t &&
        FSharpType.GetUnionCases t
            |> Seq.map (fun c -> c.GetFields().Length)
            |> Seq.forall (fun n -> n = 0)

    override x.WriteJson (writer : JsonWriter,
                          value : obj,
                          serializer : JsonSerializer) : Unit =

        let caseType = value.GetType()
        let (case, _) = FSharpValue.GetUnionFields(value, caseType)
        let json = sprintf "\"%s\"" case.Name
        writer.WriteRawValue(json)

    override x.ReadJson(reader : JsonReader,
                        t : Type,
                        existingValue : obj,
                        serializer : JsonSerializer) : obj =
        if reader.Value = null then null
        else
            let text = reader.Value.ToString()
            let cases = FSharpType.GetUnionCases t
            let case = cases |> Seq.find (fun c -> c.Name = text)
            FSharpValue.MakeUnion(case, [||])