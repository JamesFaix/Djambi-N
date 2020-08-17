module Djambi.Api.Logic.Errors

open Djambi.Api.Common.Control

let noPieceInCell<'a>() =
    GameRuleViolationException("No piece in the selected cell.")

let cellNotFound<'a>() =
    NotFoundException("Cell not found.")

let turnStatusDoesNotAllowSelection<'a>() =
    GameRuleViolationException("Cannot make selection with the current turn status.")