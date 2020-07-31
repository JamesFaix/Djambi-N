namespace Apex.Api.Common.Control

open System

type GameConfigurationException(message : string) =
    inherit Exception(message)
    
type GameRuleViolationException(message : string) =
    inherit Exception(message)

type HttpException(statusCode : int, message: string) =
    inherit Exception(message)

    member this.statusCode = statusCode
    
type NotFoundException(message : string) =
    inherit Exception(message)
