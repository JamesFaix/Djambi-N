namespace Apex.Api.Logic.Managers

open Apex.Api.Common.Control
open Apex.Api.Common.Control.AsyncHttpResult
open Apex.Api.Logic.Interfaces

type SessionManager(sessionServ : ISessionService) =
    interface ISessionManager with
        member x.login request =
            sessionServ.openSession request
            |> thenReplaceError 409 (HttpException(409, "Already signed in."))

        member x.logout session =
            sessionServ.closeSession session
            |> thenMap ignore