namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyJsonMappings
open Djambi.Api.Persistence
open Djambi.Api.Common
open System.Threading.Tasks
open System
open Djambi.Api.Http.HttpUtility
open Djambi.Api.Domain.LobbyModels
open Microsoft.Extensions.Primitives

module UserController =

    let private maxFailedLoginAttempts = 5
    let private accountLockTimeout = TimeSpan.FromHours(1.0)

    let login : HttpHandler =
        let func (ctx : HttpContext) =
            task {
                let! request = ctx.BindModelAsync<LoginRequestJsonModel>()
                              |> Task.map mapLoginRequestFromJson

                let! user = UserRepository.getUserByName request.userName

                if user.activeSessionToken.IsSome
                then raise (HttpException(401, "User already logged in"))

                let isWithinLockTimeoutPeriod (u : User) =
                    DateTime.UtcNow - u.lastFailedLoginAttemptOn.Value < accountLockTimeout

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

                ctx.Response.Headers.Add("Set-Cookie", new StringValues(""))
                
//                .SetHttpHeader("Set-Cookie", )
                    //return cookie
            }
        handle func

    let createUser : HttpHandler =
        let func (ctx : HttpContext) =            
            ctx.BindModelAsync<CreateUserJsonModel>()
            |> Task.map mapCreateUserRequest
            |> Task.bind UserRepository.createUser
            |> Task.map mapUserResponse
        handle func

    let deleteUser(userId : int) =
        let func ctx =
            UserRepository.deleteUser(userId)
        handle func

    let getUser(userId : int) =
        let func ctx =
            UserRepository.getUser userId
            |> Task.map mapUserResponse
        handle func

    let getUsers : HttpFunc -> HttpContext -> HttpContext option Task =
        let func ctx =
            UserRepository.getUsers()
            |> Task.map (Seq.map mapUserResponse)
        handle func

    let updateUser(userId : int) =
        let func ctx = 
            raise (NotImplementedException "")
        handle func
        