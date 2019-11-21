module Apex.Api.Logic.Errors

open Apex.Api.Common.Control

let noPieceInCell<'a>() : HttpResult<'a> =
    Error <| HttpException(400, "No piece in the selected cell.")

let cellNotFound<'a>() : HttpResult<'a> =
    Error <| HttpException(404, "Cell not found.")

let turnStatusDoesNotAllowSelection<'a>() : HttpResult<'a> =
    Error <| HttpException(400, "Cannot make selection with the current turn status.")