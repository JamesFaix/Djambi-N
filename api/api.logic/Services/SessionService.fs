namespace Apex.Api.Logic.Services

open System
open Apex.Api.Common.Control
open Apex.Api.Db.Interfaces
open Apex.Api.Model
open Apex.Api.Logic.Interfaces
open System.Threading.Tasks
open FSharp.Control.Tasks

type SessionService(sessionRepo : ISessionRepository,
                    userRepo : IUserRepository) =

    let maxFailedLoginAttempts = 5
    let accountLockTimeout = TimeSpan.FromHours(1.0)
    let sessionTimeout = TimeSpan.FromHours(1.0)

    let errorIfExpired (session : Session) =
        match session with
        | s when s.expiresOn >= DateTime.UtcNow -> ()
        | _ -> raise <| HttpException(401, "Session expired.")

    member x.openSession(request : LoginRequest) : Task<Session> =

        let isWithinLockTimeoutPeriod (u : UserDetails) =
            u.lastFailedLoginAttemptOn.IsNone
            || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

        let errorIfLocked (user : UserDetails) =
            if user.failedLoginAttempts >= maxFailedLoginAttempts
                && isWithinLockTimeoutPeriod user
            then raise <| HttpException(401, "Account locked.")
            else ()

        let errorIfInvalidPassword (user : UserDetails) =
            if request.password = user.password
            then Task.FromResult ()
            else
                let attempts =
                    if isWithinLockTimeoutPeriod user
                    then user.failedLoginAttempts + 1
                    else 1

                let request = UpdateFailedLoginsRequest.increment (user.id, attempts)
                task {
                    let! _ = userRepo.updateFailedLoginAttempts request
                    raise <| HttpException(401, "Incorrect password.")
                }

        let deleteSessionForUser (userId : int) : Task<unit> =            
            task {
                try 
                    let! session = sessionRepo.getSession (SessionQuery.byUserId userId)
                    return! sessionRepo.deleteSession (Some session.id, None)
                with
                | :? HttpException as ex when ex.statusCode = 404 ->
                    return () //If no session thats fine
            }

        task {
            let! user = userRepo.getUserByName request.username
            errorIfLocked user
            let! _ = errorIfInvalidPassword user

            //If a session already exists for this user, delete it
            let! _ = deleteSessionForUser user.id

            //Create a new session            
            let request : CreateSessionRequest =
                {
                    userId = user.id
                    token = Guid.NewGuid().ToString()
                    expiresOn = DateTime.UtcNow.Add(sessionTimeout)
                }
            let! session = sessionRepo.createSession request
            let! _ = userRepo.updateFailedLoginAttempts (UpdateFailedLoginsRequest.reset user.id)

            return session
        }

    member x.renewSession(token : string) : Task<Session> =
        task {
            let! session = sessionRepo.getSession (SessionQuery.byToken token)
            errorIfExpired session
            let! session = sessionRepo.renewSessionExpiration(session.id, DateTime.UtcNow.Add(sessionTimeout))
            return session            
        }

    member x.getSession(token : string) : Task<Session> =
        task {
            let! session = sessionRepo.getSession (SessionQuery.byToken token)
            errorIfExpired session
            return session
        }

    member x.closeSession(session : Session) : Task<unit> =
        sessionRepo.deleteSession(None, Some session.token)

    interface ISessionService with
        member x.openSession request = x.openSession request
        member x.renewSession token = x.renewSession token
        member x.getSession token = x.getSession token
        member x.closeSession session = x.closeSession session