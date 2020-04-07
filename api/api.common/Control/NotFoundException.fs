namespace Apex.Api.Common.Control

open System

type NotFoundException(message : string) =
    inherit Exception(message)