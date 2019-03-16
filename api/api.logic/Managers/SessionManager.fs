namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Logic.Interfaces

type SessionManager() =
    interface ISessionManager with
        member x.login request =
            SessionService.openSession request
            |> thenReplaceError 409 (HttpException(409, "Already signed in."))

        member x.logout session =
            SessionService.closeSession session
            |> thenMap ignore