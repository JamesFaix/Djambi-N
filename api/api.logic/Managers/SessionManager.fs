namespace Djambi.Api.Logic.Managers

open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Interfaces

type SessionManager(sessionServ : ISessionService) =
    interface ISessionManager with
        member x.login request =
            sessionServ.openSession request
            |> thenReplaceError 409 (HttpException(409, "Already signed in."))

        member x.logout session =
            sessionServ.closeSession session
            |> thenMap ignore