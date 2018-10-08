namespace Djambi.Api.Logic.Services

open System
open System.Threading.Tasks
open FSharp.Control.Tasks
open Djambi.Api.Common
open Djambi.Api.Db.Repositories
open Djambi.Api.Model.LobbyModel

module SessionService =

    let private maxFailedLoginAttempts = 5
    let private maxSessionUsers = 8
    let private accountLockTimeout = TimeSpan.FromHours(1.0)
    let private sessionTimeout = TimeSpan.FromHours(1.0)
    
    let signIn(username : string, password : string, token : string option) : Session Task =

        let createNewSessionWithUser(userId : int, expiresOn : DateTime) : Session Task =
            task {
                let newToken = Guid.NewGuid().ToString()
                let! session = SessionRepository.createSessionWithUser(userId, newToken, expiresOn)
                let! _ = UserRepository.updateFailedLoginAttempts(userId, 0, None)
                return session
            }

        let addUserToSession(token : string, userId: int, expiresOn : DateTime) : Session Task =
            task {
                let! tokenSession = SessionRepository.getSession(None, Some token, None)
                match tokenSession with
                | None -> 
                    raise <| HttpException(404, "Session not found")
                    return Unchecked.defaultof<Session>
                | Some s when s.userIds.Length >= maxSessionUsers ->
                    raise <| HttpException(400, "Session already has maximum number of users")
                    return Unchecked.defaultof<Session>
                | Some s ->
                    let! _ = SessionRepository.addUserToSession(s.id, userId)
                    let! updatedSession = SessionRepository.renewSessionExpiration(s.id, expiresOn)
                                          |> Task.map (fun o -> o.Value)
                    let! _ = UserRepository.updateFailedLoginAttempts(userId, 0, None)
                    return updatedSession
            }

        task {
            let! user = UserRepository.getUserByName username //Throws if user not found
            
            let isWithinLockTimeoutPeriod (u : User) =
                u.lastFailedLoginAttemptOn.IsNone
                || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

            //If account locked, error
            if user.failedLoginAttempts >= maxFailedLoginAttempts 
                && isWithinLockTimeoutPeriod user
            then raise <| HttpException(401, "Account locked")
           
            //If invalid password, error & increment failed attempts
            if password <> user.password
            then 
                let failedLoginAttempts = 
                    if isWithinLockTimeoutPeriod user
                    then user.failedLoginAttempts + 1
                    else 1

                let! _ = UserRepository.updateFailedLoginAttempts(user.id, 
                                                                  failedLoginAttempts, 
                                                                  Some DateTime.UtcNow)
                raise <| HttpException(401, "Incorrect password")

            let! userCurrentSession = SessionRepository.getSession(None, None, Some user.id)
            
            let expiresOn = DateTime.UtcNow.Add(sessionTimeout)

            if userCurrentSession.IsSome 
            then 
                match token with
                | Some t when userCurrentSession.Value.token = t ->
                    raise <| HttpException(409, "User already logged in the same session.")
                | _ -> 
                    raise <| HttpException(401, "User already logged in to another session.")

            match token with
            | Some t -> return! addUserToSession(t, user.id, expiresOn)
            | _ -> return! createNewSessionWithUser(user.id, expiresOn)
        } 

    let signOut(userId : int, token : string) : Session option Task =
        task {
            let! userSession = SessionRepository.getSession(None, None, Some userId)
            
            if userSession.IsNone
            then raise <| HttpException(403, "User is not signed in")

            let s = userSession.Value

            if (s.token <> token)
            then raise <| HttpException(403, "Invalid token")

            return! SessionRepository.removeUserFromSession(s.id, userId)
        } 

    let renewSession(token : string) : Session Task =
        task {

            let! session = SessionRepository.getSession(None, Some token, None)
            if session.IsNone
            then raise <| HttpException(404, "Session not found")

            let s = session.Value

            if s.expiresOn < DateTime.UtcNow
            then raise <| HttpException(403, "Session expired")

            let! updatedSession = SessionRepository.renewSessionExpiration(s.id, DateTime.UtcNow.Add(sessionTimeout))
            return updatedSession.Value            
        } 

    let getSession(token : string) : Session option Task =
        let someIfNotExpired session =
             if session.expiresOn >= DateTime.UtcNow 
             then Some session 
             else None

        task {
            return! SessionRepository.getSession(None, Some token, None)
                    |> Task.map (Option.bind someIfNotExpired)
        }