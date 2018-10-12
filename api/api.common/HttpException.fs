namespace Djambi.Api.Common

open System
open System.Threading.Tasks

type HttpException(statusCode : int, message: string) =
    inherit Exception(message)

    member this.statusCode = statusCode
    
type AsyncHttpResult<'a> = Task<Result<'a, HttpException>>