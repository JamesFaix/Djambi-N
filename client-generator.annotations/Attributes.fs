namespace Djambi.ClientGenerator.Annotations

open System

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Enum ||| AttributeTargets.Struct)>]
type ClientTypeAttribute() =
    inherit Attribute()