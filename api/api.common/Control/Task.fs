//Intentionally not AutoOpen
module Apex.Api.Common.Control.Task

open System.Threading.Tasks
open FSharp.Control.Tasks

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

let toGeneric (t : Task) : unit Task =
    t.ContinueWith<unit>(ignore, TaskContinuationOptions.OnlyOnRanToCompletion)