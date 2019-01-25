module Djambi.Api.Logic.Services.SecurityService

open Djambi.Api.Common.Control
open Djambi.Api.Model

let notAdminErrorMessage = "You must have admin privileges to complete the requested action."

let notAdminOrCreatorErrorMessage = "You must have admin privileges or be the game's creator to complete the requested action."

let notAdminOrPlayerErrorMessage = "You must have admin privileges or be a player in the game to complete the requested action."
    
let notAdminOrCurrentPlayerErrorMessage = "You must have admin privileges or be the current player in the game to complete the requested action."

let notAdminOrSelfErrorMessage = "You must have admin privileges or be the target user to complete the requested action."

let ensureAdmin (session : Session) : Unit HttpResult =
    if session.isAdmin
    then Ok ()
    else Error <| HttpException(403, notAdminErrorMessage)

let ensureAdminOrCreator (session : Session) (game : Game) : Unit HttpResult =
    if session.isAdmin 
        || session.userId = game.createdByUserId
    then Ok ()
    else Error <| HttpException(403, notAdminOrCreatorErrorMessage)

let ensureAdminOrPlayer (session : Session) (game : Game) : Unit HttpResult =
    if session.isAdmin
        || game.players |> List.exists (fun p -> p.userId = Some session.userId)
    then Ok ()
    else Error <| HttpException(403, notAdminOrPlayerErrorMessage)

let ensureAdminOrCurrentPlayer (session : Session) (game : Game) : Unit HttpResult =
    let pass = 
        if session.isAdmin
        then true
        elif not game.turnCycle.IsEmpty
        then         
            let currentPlayer = game.players |> List.find (fun p -> p.id = game.turnCycle.Head)
            currentPlayer.userId = Some session.userId
        else false

    if pass then Ok ()
    else Error <| HttpException(403, notAdminOrCurrentPlayerErrorMessage)

let ensureAdminOrSelf (session : Session) (userId : int) : Unit HttpResult =
    if session.isAdmin
        || session.userId = userId
    then Ok ()
    else Error <| HttpException(403, notAdminOrSelfErrorMessage)