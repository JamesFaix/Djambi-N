namespace Djambi.Api.Logic.Services

open System
open System.Threading.Tasks
open Djambi.Api.Common
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel

module SessionService =

    let private maxFailedLoginAttempts = 5
    let private maxSessionUsers = 8
    let private accountLockTimeout = TimeSpan.FromHours(1.0)
    let private sessionTimeout = TimeSpan.FromHours(1.0)
    
    let signIn(username : string, password : string, token : string option) : Session AsyncHttpResult =

        let createNewSessionWithUser(userId : int, expiresOn : DateTime) : Session AsyncHttpResult =
            let newToken = Guid.NewGuid().ToString()

            SessionRepository.createSessionWithUser(userId, newToken, expiresOn)
            |> Task.thenDoAsync (fun _ -> UserRepository.updateFailedLoginAttempts(userId, 0, None))

        let addUserToSession(token : string, userId: int, expiresOn : DateTime) : Session AsyncHttpResult =
            let errorIfMaxUsers (session : Session) =
                if session.userIds.Length >= maxSessionUsers
                then Error <| HttpException(400, "Session already has maximum number of users")
                else Ok session

            SessionRepository.getSession(None, Some token, None)
            |> Task.thenBind errorIfMaxUsers
            |> Task.thenBindAsync (fun s -> SessionRepository.addUserToSession(s.id, userId))
            |> Task.thenBindAsync (fun s -> SessionRepository.renewSessionExpiration(s.id, expiresOn))
            |> Task.thenDoAsync (fun _ -> UserRepository.updateFailedLoginAttempts(userId, 0, None))
         
        let isWithinLockTimeoutPeriod (u : User) =
            u.lastFailedLoginAttemptOn.IsNone
            || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout
        
        let errorIfLocked (user : User) =
            if user.failedLoginAttempts >= maxFailedLoginAttempts 
                && isWithinLockTimeoutPeriod user
            then Error <| HttpException(401, "Account locked")
            else Ok user

        let errorIfInvalidPassword (user : User) =
            if password = user.password
            then Task.FromResult(Ok user)
            else 
                let failedLoginAttempts = 
                    if isWithinLockTimeoutPeriod user
                    then user.failedLoginAttempts + 1
                    else 1

                UserRepository.updateFailedLoginAttempts(user.id, failedLoginAttempts, Some DateTime.UtcNow)
                |> Task.thenBind (fun _ -> Error <| HttpException(401, "Incorrect password"))

        let errorIfLoggedIn (user : User) =
            SessionRepository.getSession(None, None, Some user.id)            
            |> Task.thenBind (fun _ -> Error <| HttpException(409, "User already logged in"))
            |> Task.thenBindError 404 (fun _ -> Ok user)

        let createOrAddToSession (user : User) =
            let expiresOn = DateTime.UtcNow.Add(sessionTimeout)
            match token with
            | Some t -> addUserToSession(t, user.id, expiresOn)
            | _ -> createNewSessionWithUser(user.id, expiresOn)

        UserRepository.getUserByName username
        |> Task.thenBind errorIfLocked
        |> Task.thenBindAsync errorIfInvalidPassword
        |> Task.thenBindAsync errorIfLoggedIn
        |> Task.thenBindAsync createOrAddToSession

    let removeUserFromSession(userId : int, token : string) : Session AsyncHttpResult =
        let errorIfTokenDoesntMatch (session : Session) =
            match session.token with
            | token -> Ok session
            | _ -> Error <| HttpException(403, "Invalid token")

        let remove (session : Session) =
            SessionRepository.removeUserFromSession(session.id, userId)

        SessionRepository.getSession(None, None, Some userId)
        |> Task.thenReplaceError 404 (HttpException(403, "User is not signed in"))
        |> Task.thenBind errorIfTokenDoesntMatch
        |> Task.thenBindAsync remove        

    let private errorIfExpired (session : Session) =
        match session with 
        | s when s.expiresOn >= DateTime.UtcNow -> Ok session
        | _ -> Error <| HttpException(403, "Session expired")

    let renewSession(token : string) : Session AsyncHttpResult =        
        let renew (s : Session) =
            SessionRepository.renewSessionExpiration(s.id, DateTime.UtcNow.Add(sessionTimeout))

        SessionRepository.getSession(None, Some token, None)
        |> Task.thenBind errorIfExpired
        |> Task.thenBindAsync renew

    let getSession(token : string) : Session AsyncHttpResult =        
        SessionRepository.getSession(None, Some token, None)
        |> Task.thenBind errorIfExpired

    let closeSession(token : string) : Unit AsyncHttpResult =
        SessionRepository.deleteSession(None, Some token)