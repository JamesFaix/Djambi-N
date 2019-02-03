module Djambi.Api.UnitTests.JsonConverterTests

open Newtonsoft.Json
open Xunit
open Djambi.Api.Common.Json
open Djambi.Api.Model

//---Option---

type RecordWithOption = { age : int option }

[<Fact>]
let ``Default None serialization is null``() =
    let record : RecordWithOption = { age = None }
    let expected = """{"age":null}"""

    let actual = JsonConvert.SerializeObject(record);

    Assert.Equal(expected, actual)

[<Fact>]
let ``Default Some serialization is awkward``() =
    let record : RecordWithOption = { age = Some 3 }
    let expected = """{"age":{"Case":"Some","Fields":[3]}}"""

    let actual = JsonConvert.SerializeObject(record);

    Assert.Equal(expected, actual)

[<Fact>]
let ``OptionJsonConverter None should serialize as null``() =
    let converter = OptionJsonConverter() :> JsonConverter 
    let record : RecordWithOption = { age = None }
    let expected = """{"age":null}""" 

    let actual = JsonConvert.SerializeObject(record, [|converter|])

    Assert.Equal(expected, actual)

[<Fact>]
let ``OptionJsonConverter Some should serialize as value``() =
    let converter = OptionJsonConverter() :> JsonConverter 
    let record : RecordWithOption = { age = Some 3 }
    let expected = """{"age":3}""" 

    let actual = JsonConvert.SerializeObject(record, [|converter|])

    Assert.Equal(expected, actual)

[<Fact>]
let ``OptionJsonConverter null should deserialize as None``() =
    let converter = OptionJsonConverter() :> JsonConverter 
    let json = """{"age":null}""" 
    let expected : RecordWithOption = { age = None }

    let actual = JsonConvert.DeserializeObject<RecordWithOption>(json, [|converter|])

    Assert.Equal(expected, actual)

[<Fact>]
let ``OptionJsonConverter non-null should deserialize as Some``() =
    let converter = OptionJsonConverter() :> JsonConverter 
    let json = """{"age":3}""" 
    let expected : RecordWithOption = { age = Some 3 }

    let actual = JsonConvert.DeserializeObject<RecordWithOption>(json, [|converter|])

    Assert.Equal(expected, actual)

//---Tuple---

[<Fact>]
let ``Default tuple serialization is awkward``() =
    let tuple = (1, true, "hello")
    let expected = """{"Item1":1,"Item2":true,"Item3":"hello"}"""

    let actual = JsonConvert.SerializeObject(tuple)

    Assert.Equal(expected, actual)

[<Fact>]
let ``TupleArrayJsonConverter tuple should serialize as array``() =
    let converter = TupleArrayJsonConverter() :> JsonConverter
    let tuple = (1, true, "hello")
    let expected = """[1,true,"hello"]"""

    let actual = JsonConvert.SerializeObject(tuple, [|converter|])

    Assert.Equal(expected, actual)

[<Fact>]
let ``TupleArrayJsonConverter array should deserialize as tuple``() =
    let converter = TupleArrayJsonConverter() :> JsonConverter
    let json = """[1,true,"hello"]"""
    let expected = (1, true, "hello")
 
    let actual = JsonConvert.DeserializeObject<int * bool * string>(json, [|converter|])

    Assert.Equal(expected, actual)
    
//---Enum union---

type UnionEnum = Yes | No | Maybe
type RecordWithEnum = { rsvp : UnionEnum }

[<Fact>]
let ``Default union enum serialization is awkward``() =
    let record = { rsvp = Maybe }
    let expected = """{"rsvp":{"Case":"Maybe"}}"""

    let actual = JsonConvert.SerializeObject(record)

    Assert.Equal(expected, actual)

[<Fact>]
let ``UnionEnumJsonConverter enum should serialize as string``() =
    let converter = UnionEnumJsonConverter() :> JsonConverter
    let record : RecordWithEnum = { rsvp = Maybe }
    let expected = """{"rsvp":"Maybe"}"""

    let actual = JsonConvert.SerializeObject(record, [|converter|])

    Assert.Equal(expected, actual)

[<Fact>]
let ``UnionEnumJsonConverter string should deserialized as enum``() =
    let converter = UnionEnumJsonConverter() :> JsonConverter
    let json = """{"rsvp":"Maybe"}"""
    let expected : RecordWithEnum = { rsvp = Maybe }

    let actual = JsonConvert.DeserializeObject<RecordWithEnum>(json, [|converter|])

    Assert.Equal(expected, actual)


//---Record union---

type RecordA = { name : string }
type RecordB = { height : int; weight : int }
type RecordUnion = A of RecordA | B of RecordB
type TupleUnion = C of int | D of int * string

[<Fact>]
let ``Default record union serialization is awkward``() =
    let record : RecordUnion = A { name = "test" }
    let expected = """{"Case":"A","Fields":[{"name":"test"}]}"""

    let actual = JsonConvert.SerializeObject(record)

    Assert.Equal(expected, actual)

[<Fact>]
let ``Default tuple union serialization is awkward``() =
    let record : TupleUnion = D(1, "test")
    let expected = """{"Case":"D","Fields":[1,"test"]}"""

    let actual = JsonConvert.SerializeObject(record)

    Assert.Equal(expected, actual)

[<Fact>]
let ``SingleFieldUnionJsonConverter should serialize union where all cases have 1 field``() =
    let converter = SingleFieldUnionJsonConverter() :> JsonConverter
    let record : RecordUnion = A { name = "test" }
    let expected = """{"kind":"A","value":{"name":"test"}}"""

    let actual = JsonConvert.SerializeObject(record, [|converter|])

    Assert.Equal(expected, actual)

[<Fact>]
let ``SingleFieldUnionJsonConverter should deserialize union where all cases have 1 field``() =
    let converter = SingleFieldUnionJsonConverter() :> JsonConverter
    let json = """{"kind":"A","value":{"name":"test"}}"""
    let expected : RecordUnion = A { name = "test" }

    let actual = JsonConvert.DeserializeObject<RecordUnion>(json, [|converter|])

    Assert.Equal(expected, actual)
//---Combos---

[<Fact>]
let ``Should serialize None of union enum``() =
    let converters = 
        [|
            UnionEnumJsonConverter() :> JsonConverter    
            OptionJsonConverter() :> JsonConverter
        |]

    let opt : Option<UnionEnum> = None
    let expected = "null"

    let actual = JsonConvert.SerializeObject(opt, converters)

    Assert.Equal(expected, actual)


[<Fact>]
let ``Should deserialize None of union enum``() =
    let converters = 
        [|
            UnionEnumJsonConverter() :> JsonConverter    
            OptionJsonConverter() :> JsonConverter
        |]

    let json = "null"
    let expected : Option<UnionEnum> = None

    let actual = JsonConvert.DeserializeObject<Option<UnionEnum>>(json, converters)

    Assert.Equal(expected, actual)

[<Fact>]
let ``Should deserialize Union with Record case with Record property``() =
    let converters = 
        [|
            UnionEnumJsonConverter() :> JsonConverter    
            OptionJsonConverter() :> JsonConverter
            SingleFieldUnionJsonConverter() :> JsonConverter
        |]

    let json = """{"kind":"PlayerAdded","value":{"playerRequest":{"kind":"Guest","userId":7,"name":"test"}}}"""
    let expectedRequest : CreatePlayerRequest = 
        { 
            kind = PlayerKind.Guest
            userId = Some 7
            name = Some "test"
        }
    let expected = Effect.PlayerAdded { playerRequest = expectedRequest }

    let actual = JsonConvert.DeserializeObject<Effect>(json, converters)

    Assert.Equal(expected, actual)