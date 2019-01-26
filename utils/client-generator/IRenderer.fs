namespace Djambi.ClientGenerator

open System

type IRenderer =
    abstract member renderType : Type -> string