//Intentionally not AutoOpen
module Djambi.Api.Common.Control.Task

open System.Threading.Tasks

let toGeneric (t : Task) : unit Task =
    t.ContinueWith<unit>(ignore, TaskContinuationOptions.OnlyOnRanToCompletion)