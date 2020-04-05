namespace Apex.Api.Logic.Managers

open Apex.Api.Common.Control
open Apex.Api.Logic.Interfaces
open FSharp.Control.Tasks

type SessionManager(sessionServ : ISessionService) =
    interface ISessionManager with
        member x.login request =
            task {
                try
                    return! sessionServ.openSession request
                with
                | :? HttpException as ex when ex.statusCode = 409 ->
                    return raise <| HttpException(409, "Already signed in.")
            }

        member x.logout session =
            task {
                return! sessionServ.closeSession session            
            }