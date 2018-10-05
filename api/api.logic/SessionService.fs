namespace Djambi.Api.Domain

open System
open System.Threading.Tasks
open FSharp.Control.Tasks

open Djambi.Api.Common
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Persistence

module SessionService =

    let private maxFailedLoginAttempts = 5
    let private accountLockTimeout = TimeSpan.FromHours(1.0)
    let private sessionTimeout = TimeSpan.FromHours(1.0)

    let signIn (request : LoginRequest) : (string * DateTimeOffset) Task =
        task {
            let! user = UserRepository.getUserByName request.userName

            let! hasSession = UserRepository.userHasSession user.id
            if hasSession
            then raise (HttpException(401, "User already logged in somewhere else."))

            let isWithinLockTimeoutPeriod (u : User) =
                u.lastFailedLoginAttemptOn.IsNone
                || DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

            if user.failedLoginAttempts > maxFailedLoginAttempts
            && isWithinLockTimeoutPeriod user
            then raise (HttpException(401, "Account locked"))
                
            if request.password <> user.password
            then
                let failedLoginAttempts = 
                    if isWithinLockTimeoutPeriod user
                    then user.failedLoginAttempts + 1
                    else 1

                let! _ = UserRepository.updateFailedLoginAttempts(user.id, 
                                                                  failedLoginAttempts, 
                                                                  Some DateTime.UtcNow)
                raise (HttpException(401, "Incorrect password"))

            let sessionToken = Guid.NewGuid().ToString()
                
            let! _ = UserRepository.createSession(user.id, 
                                                  sessionToken, 
                                                  DateTime.UtcNow.Add(sessionTimeout))

            return (sessionToken, DateTimeOffset.UtcNow.Add(sessionTimeout))
        }

    let signOut (user : User) =
        UserRepository.deleteSession user.id        