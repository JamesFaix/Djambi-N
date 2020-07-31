namespace Apex.Api.Common.Control

open System

type ApexWebsocketException(message : string) =
    inherit Exception(message)

type GameConfigurationException(message : string) =
    inherit Exception(message)
    
type GameRuleViolationException(message : string) =
    inherit Exception(message)

type InvalidWebRequestException(message : string) =
    inherit Exception(message)

type NotFoundException(message : string) =
    inherit Exception(message)
