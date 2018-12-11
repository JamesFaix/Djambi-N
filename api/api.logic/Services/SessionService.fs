namespace Djambi.Api.Logic.Services

open System
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model

module SessionService =

    let private maxFailedLoginAttempts = 5
    let private accountLockTimeout = TimeSpan.FromHours(1.0)
    let private sessionTimeout = TimeSpan.FromHours(1.0)

    let openSession(request : LoginRequest) : Session AsyncHttpResult =

        let isWithinLockTimeoutPeriod (u : UserDetails) =
            u.lastFailedLoginAttemptOn.IsNone
            || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

        let errorIfLocked (user : UserDetails) =
            if user.failedLoginAttempts >= maxFailedLoginAttempts
                && isWithinLockTimeoutPeriod user
            then Error <| HttpException(401, "Account locked.")
            else Ok user

        let errorIfInvalidPassword (user : UserDetails) =
            if request.password = user.password
            then okTask user
            else
                let attempts = 
                    if isWithinLockTimeoutPeriod user
                    then user.failedLoginAttempts + 1
                    else 1

                let request = UpdateFailedLoginsRequest.increment (user.id, attempts)

                UserRepository.updateFailedLoginAttempts request
                |> thenBind (fun _ -> Error <| HttpException(401, "Incorrect password."))

        UserRepository.getUserByName request.username
        |> thenBind errorIfLocked
        |> thenBindAsync errorIfInvalidPassword
        |> thenBindAsync (fun user ->
            //If a session already exists for this user, delete it
            SessionRepository.getSession (SessionQuery.byUserId user.id)
            |> thenBindAsync(fun session -> SessionRepository.deleteSession (Some session.id, None))
            |> thenBindError 404 (fun _ -> Ok ()) //If no session thats fine
            //Create a new session
            |> thenBindAsync(fun _ ->
                let request : CreateSessionRequest =
                    {
                        userId = user.id
                        token = Guid.NewGuid().ToString()
                        expiresOn = DateTime.UtcNow.Add(sessionTimeout)
                    }
                SessionRepository.createSession request
            )
            |> thenDoAsync (fun _ -> 
                UserRepository.updateFailedLoginAttempts (UpdateFailedLoginsRequest.reset user.id)                
            )
        )

    let private errorIfExpired (session : Session) =
        match session with
        | s when s.expiresOn >= DateTime.UtcNow -> Ok session
        | _ -> Error <| HttpException(403, "Session expired.")

    let renewSession(token : string) : Session AsyncHttpResult =
        let renew (s : Session) =
            SessionRepository.renewSessionExpiration(s.id, DateTime.UtcNow.Add(sessionTimeout))

        SessionRepository.getSession (SessionQuery.byToken token)
        |> thenBind errorIfExpired
        |> thenBindAsync renew

    let getSession(token : string) : Session AsyncHttpResult =
        SessionRepository.getSession (SessionQuery.byToken token)
        |> thenBind errorIfExpired

    let closeSession(session : Session) : Unit AsyncHttpResult =
        SessionRepository.deleteSession(None, Some session.token)