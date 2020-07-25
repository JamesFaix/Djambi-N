module Apex.Api.Logic.Errors

open Apex.Api.Common.Control

let noPieceInCell<'a>() =
    HttpException(400, "No piece in the selected cell.")

let cellNotFound<'a>() =
    HttpException(404, "Cell not found.")

let turnStatusDoesNotAllowSelection<'a>() =
    HttpException(400, "Cannot make selection with the current turn status.")