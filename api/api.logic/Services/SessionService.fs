namespace Djambi.Api.Logic.Services

open System
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.SessionModel
open Djambi.Api.Model.UserModel

module SessionService =

    let private maxFailedLoginAttempts = 5
    let private accountLockTimeout = TimeSpan.FromHours(1.0)
    let private sessionTimeout = TimeSpan.FromHours(1.0)

    let openSession(request : LoginRequest) : Session AsyncHttpResult =

        let isWithinLockTimeoutPeriod (u : User) =
            u.lastFailedLoginAttemptOn.IsNone
            || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

        let errorIfLocked (user : User) =
            if user.failedLoginAttempts >= maxFailedLoginAttempts
                && isWithinLockTimeoutPeriod user
            then Error <| HttpException(401, "Account locked.")
            else Ok user

        let errorIfInvalidPassword (user : User) =
            if request.password = user.password
            then okTask user
            else
                let failedLoginAttempts =
                    if isWithinLockTimeoutPeriod user
                    then user.failedLoginAttempts + 1
                    else 1

                UserRepository.updateFailedLoginAttempts(user.id, failedLoginAttempts, Some DateTime.UtcNow)
                |> thenBind (fun _ -> Error <| HttpException(401, "Incorrect password."))

        let errorIfLoggedIn (user : User) =
            SessionRepository.getSession(None, None, Some user.id)
            |> thenBind (fun _ -> Error <| HttpException(409, "Already signed in."))
            |> thenBindError 404 (fun _ -> Ok user)

        UserRepository.getUserByName request.username
        |> thenBind errorIfLocked
        |> thenBindAsync errorIfInvalidPassword
        |> thenBindAsync errorIfLoggedIn
        |> thenBindAsync (fun user ->
            let sessionRequest =
                {
                    userId = user.id
                    token = Guid.NewGuid().ToString()
                    expiresOn = DateTime.UtcNow.Add(sessionTimeout)
                }
            SessionRepository.createSession sessionRequest
            |> thenDoAsync (fun _ -> UserRepository.updateFailedLoginAttempts(user.id, 0, None))
        )

    let private errorIfExpired (session : Session) =
        match session with
        | s when s.expiresOn >= DateTime.UtcNow -> Ok session
        | _ -> Error <| HttpException(403, "Session expired.")

    let renewSession(token : string) : Session AsyncHttpResult =
        let renew (s : Session) =
            SessionRepository.renewSessionExpiration(s.id, DateTime.UtcNow.Add(sessionTimeout))

        SessionRepository.getSession(None, Some token, None)
        |> thenBind errorIfExpired
        |> thenBindAsync renew

    let getSession(token : string) : Session AsyncHttpResult =
        SessionRepository.getSession(None, Some token, None)
        |> thenBind errorIfExpired

    let closeSession(session : Session) : Unit AsyncHttpResult =
        SessionRepository.deleteSession(None, Some session.token)