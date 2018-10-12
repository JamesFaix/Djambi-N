namespace Djambi.Api.Common

open System.Threading.Tasks
open FSharp.Control.Tasks

module Task =
    
    let map<'a, 'b> (projection : 'a -> 'b) (t : 'a Task) : 'b Task =
        task {
            let! result = t
            return projection result
        }
    
    let bind<'a, 'b> (projection : 'a -> 'b Task) (t : 'a Task) : 'b Task =
        task {
            let! result = t
            return! projection result
        }

    let thenMap (projection : 'a -> 'b) (t : 'a HttpResult Task) : 'b HttpResult Task =
        t |> map (Result.map projection)

    let thenBind (projection : 'a -> 'b HttpResult) (t : 'a HttpResult Task) : 'b HttpResult Task =
        t |> map (Result.bind projection)

    let thenBindAsync (projection : 'a -> 'b HttpResult Task) (t : 'a HttpResult Task) : 'b HttpResult Task =
        task {
            let! result = t
            match result with
            | Ok x -> 
                return! projection x
            | Error x -> 
                return Error x                
        }