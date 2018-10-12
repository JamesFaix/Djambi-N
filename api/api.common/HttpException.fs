namespace Djambi.Api.Common

open System
open System.Threading.Tasks

type HttpException(statusCode : int, message: string) =
    inherit Exception(message)

    member this.statusCode = statusCode

type HttpResult<'a> = Result<'a, HttpException>

type AsyncHttpResult<'a> = Task<HttpResult<'a>>