namespace Djambi.Api.Common

open System
open System.Threading.Tasks
open Giraffe

type HttpException(statusCode : int, message: string) =
    inherit Exception(message)

    member this.statusCode = statusCode

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
