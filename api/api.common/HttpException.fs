namespace Djambi.Api.Common

open System

type HttpException(statusCode : int, message: string) =
    inherit Exception(message)

    member this.statusCode = statusCode