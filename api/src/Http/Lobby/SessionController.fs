namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common
open System
open Djambi.Api.Http.HttpUtility
open Djambi.Api.Domain.LobbyModels
open Djambi.Api.Common.Utilities

module SessionController =

    let private maxFailedLoginAttempts = 5
    let private accountLockTimeout = TimeSpan.FromHours(1.0)

    let signIn : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                let! request = ctx.BindModelAsync<LoginRequestJsonModel>()
                              |> Task.map mapLoginRequestFromJson

                let! user = UserRepository.getUserByName request.userName

                if user.activeSessionToken.IsSome
                then raise (HttpException(401, "User already logged in"))

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

                    let! _ = UserRepository.updateUserLoginData(user.id, 
                                                                failedLoginAttempts, 
                                                                Some DateTime.UtcNow, 
                                                                None)
                    raise (HttpException(401, "Incorrect password"))

                let sessionToken = Guid.NewGuid().ToString()
                
                let! _ = UserRepository.updateUserLoginData(user.id,
                                                            0,
                                                            None,
                                                            Some sessionToken)
                
                let cookieOptions = new CookieOptions()
                cookieOptions.Domain <- "localhost"
                cookieOptions.Path <- "/"
                cookieOptions.Secure <- false
                cookieOptions.HttpOnly <- true
                cookieOptions.Expires <- DateTimeOffset.UtcNow.AddHours(1.0) |> toNullable

                ctx.Response.Cookies.Append("DjambiSession", sessionToken, cookieOptions);
            }
        handle func

    let signOut : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                let cookie = ctx.Request.Cookies.Item("DjambiSession");
                let! user = UserRepository.getUserBySession cookie
                return! UserRepository.updateUserLoginData(user.id, 0, None, None)
            }
        handle func
        