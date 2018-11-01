module Djambi.Api.Common.Result

let isOk (r : Result<'a, 'b>) =
    match r with
    | Ok _ -> true
    | _ -> false

let isError (r : Result<'a, 'b>) =
    match r with
    | Ok _ -> false
    | _ -> true

let value (r : Result<'a, 'b>) =
    match r with
    | Ok x -> x
    | _ -> raise <| invalidOp "Cannot get value of error result."

let error (r : Result<'a, 'b>) =
    match r with
    | Error x -> x
    | _ -> raise <| invalidOp "Cannot get error of ok result."