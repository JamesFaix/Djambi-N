//Intentionally not AutoOpen
module Djambi.Api.Common.Control.Result

open System.Threading.Tasks

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

let bindAsync<'a, 'b, 'c> (projection : 'a -> Task<Result<'b, 'c>>) (r : Result<'a, 'c>) : Task<Result<'b, 'c>> =
    match r with
    | Ok x -> projection x
    | Error x -> Task.FromResult(Error x)