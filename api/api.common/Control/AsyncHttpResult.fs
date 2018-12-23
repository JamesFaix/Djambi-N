namespace Djambi.Api.Common.Control

open System.Threading.Tasks
open FSharp.Control.Tasks

type HttpResult<'a> = Result<'a, HttpException>

type AsyncHttpResult<'a> = Task<HttpResult<'a>>

//Intentionally not AutoOpen
module AsyncHttpResult =

    let thenMap
        (projection : 'a -> 'b)
        (t : 'a AsyncHttpResult)
        : 'b AsyncHttpResult =

        t |> Task.map (Result.map projection)

    let thenBind
        (projection : 'a -> Result<'b, HttpException>)
        (t : 'a AsyncHttpResult)
        : 'b AsyncHttpResult =

        t |> Task.map (Result.bind projection)

    let thenBindAsync
        (projection : 'a -> 'b AsyncHttpResult)
        (t : 'a AsyncHttpResult)
        : 'b AsyncHttpResult =

        let projectIfValue (result : Result<'a, HttpException>) =
            match result with
            | Ok x -> projection x
            | Error x -> Task.FromResult(Error x)

        t |> Task.bind projectIfValue

    let thenDoAsync
        (action : 'a -> _ AsyncHttpResult)
        (t : 'a AsyncHttpResult)
        : 'a AsyncHttpResult =

        task {
            let! result = t
            match result with
            | Error _ -> return result
            | Ok x ->
                let! actionResult = action x
                match actionResult with
                | Ok _ -> return Ok x
                | Error y -> return Error y
        }

    let thenDoEachAsync
        (action : 'a -> Unit AsyncHttpResult)
        (t : 'a seq AsyncHttpResult)
        : Unit AsyncHttpResult =

        let doEachAsync (items : 'a seq) : Unit AsyncHttpResult =
            task {
                let mutable result = Ok ()
                let mutable stop = false

                use e = items.GetEnumerator()
                while not stop && e.MoveNext() do
                    let! res = action e.Current
                    match res with
                    | Error _ -> result <- res
                                 stop <- true
                    | _ -> ()

                return result
            }

        t |> thenBindAsync doEachAsync

    let applyEachAsync<'a>
        (projections : seq<'a -> 'a AsyncHttpResult>)
        (seed : 'a)
        : 'a AsyncHttpResult =
        task {
            let mutable result = Ok seed
            let mutable stop = false
            
            use e = projections.GetEnumerator()
            while not stop && e.MoveNext() do
                let projection = e.Current
                let input = result |> Result.value
                let! res = projection input
                result <- res

                if Result.isError res 
                then stop <- true
                else ()

            return result        
        }

    let thenReplaceError
        (statusCode : int)
        (newException : HttpException)
        (t : 'a AsyncHttpResult)
        : 'a AsyncHttpResult =

        let mapIfMatch (oldException : HttpException) =
            match oldException with
            | ex when ex.statusCode = statusCode -> newException
            | _ -> oldException

        t |> Task.map (Result.mapError mapIfMatch)

    let thenBindError
        (statusCode : int)
        (projection : HttpException -> Result<'a, HttpException>)
        (t : 'a AsyncHttpResult)
        : 'a AsyncHttpResult =

        task {
            let! result = t
            match result with
            | Error ex when ex.statusCode = statusCode ->
                return projection ex
            | _ -> return result
        }

    let thenBindErrorAsync
        (statusCode : int)
        (projection : HttpException -> 'a AsyncHttpResult)
        (t : 'a AsyncHttpResult)
        : 'a AsyncHttpResult =

        task {
            let! result = t
            match result with
            | Error ex when ex.statusCode = statusCode ->
                return! projection ex
            | _ -> return result
        }

    let thenValue (t : 'a AsyncHttpResult) : 'a Task =
        t |> Task.map Result.value

    let thenError (t : 'a AsyncHttpResult) : HttpException Task =
        t |> Task.map Result.error

    let okTask (value : 'a): 'a AsyncHttpResult =
        value |> Ok |> Task.FromResult

    let errorTask (ex : HttpException) : 'a AsyncHttpResult =
        ex |> Error |> Task.FromResult