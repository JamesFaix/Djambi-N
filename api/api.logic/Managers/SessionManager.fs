namespace Djambi.Api.Logic.Managers

open Djambi.Api.Logic.Interfaces

type SessionManager(sessionServ : ISessionService) =
    interface ISessionManager with
        member __.login request =
            sessionServ.openSession request

        member __.logout session =
            sessionServ.closeSession session