namespace Djambi.ClientGenerator

open System
open System.Reflection

type IRenderer =
    abstract member name : string with get

    abstract member modelOutputPathSetting : string with get

    abstract member endpointsOutputPathSetting : string with get
    
    abstract member renderModel : Type list -> string

    abstract member renderFunctions : MethodInfo list -> string