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
