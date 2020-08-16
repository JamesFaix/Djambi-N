module Djambi.Api.Logic.Security

open Djambi.Api.Model
open Djambi.Api.Enums
open System

let noPrivilegeErrorMessage = "You do not have the required privilege to complete the requested action."

let noPrivilegeOrCreatorErrorMessage = "You must be the game's creator or have the required privilege to complete the requested action."

let noPrivilegeOrPlayerErrorMessage = "You must be a player in the game or have the required privilege to complete the requested action."

let noPrivilegeOrCurrentPlayerErrorMessage = "You must be the current player in the game or have the required privilege to complete the requested action."

let noPrivilegeOrSelfErrorMessage = "You must be the target user or have the required privilege to complete the requested action."

let ensureHas (privilege : Privilege) (session : Session) : Unit =
    if session.user.has privilege
    then ()
    else raise <| UnauthorizedAccessException(noPrivilegeErrorMessage)

let ensureCreatorOrEditPendingGames (session : Session) (game : Game) : Unit =
    let self = session.user
    if self.has Privilege.EditPendingGames
        || self.id = game.createdBy.userId
    then ()
    else raise <| UnauthorizedAccessException(noPrivilegeOrCreatorErrorMessage)

let ensurePlayerOrHas (privilege : Privilege) (session : Session) (game : Game) : Unit =
    let self = session.user
    if self.has privilege
        || game.players |> List.exists (fun p -> p.userId = Some self.id)
    then ()
    else raise <| UnauthorizedAccessException(noPrivilegeOrPlayerErrorMessage)

let ensureCurrentPlayerOrOpenParticipation (session : Session) (game : Game) : Unit =
    let self = session.user
    let pass =
        if self.has Privilege.OpenParticipation
        then true
        elif not game.turnCycle.IsEmpty
        then
            let currentPlayer = game.players |> List.find (fun p -> p.id = game.turnCycle.Head)
            currentPlayer.userId = Some self.id
        else false

    if pass then ()
    else raise <| UnauthorizedAccessException(noPrivilegeOrCurrentPlayerErrorMessage)

let ensureSelfOrHas (privilege : Privilege) (session : Session) (userId : int) : Unit =
    let self = session.user
    if self.has privilege
        || self.id = userId
    then ()
    else raise <| UnauthorizedAccessException(noPrivilegeOrSelfErrorMessage)