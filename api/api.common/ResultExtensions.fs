module Djambi.Api.Common.ResultExtensions

type Result<'a, 'b> with
    member this.IsOk() =
        match this with
        | Ok _ -> true
        | _ -> false

    member this.IsError() =
        match this with
        | Ok _ -> false
        | _ -> true

    member this.Value() =
        match this with
        | Ok x -> x
        | _ -> raise <| invalidOp "Cannot get value of error result."

    member this.Error() =
        match this with
        | Error x -> x
        | _ -> raise <| invalidOp "Cannot get error string of ok result."