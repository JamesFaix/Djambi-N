namespace Djambi.ClientGenerator

open System

type IRenderer =
    abstract member renderTypes : Type list -> string