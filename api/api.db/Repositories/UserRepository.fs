namespace Apex.Api.Db.Repositories

open Apex.Api.Db.Interfaces
open System
open Apex.Api.Db.Model
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open Apex.Api.Db.Mappings
open Microsoft.EntityFrameworkCore

type UserRepository(context : ApexDbContext) =    
    let nameConflictMessage = 
        "The instance of entity type 'UserSqlModel' cannot be tracked because " + 
        "another instance with the same key value for {'Name'} is already being tracked."

    interface IUserRepository with
        member __.getUser userId =
            task {
                let! sqlModel = context.Users.FindAsync(userId)

                return match sqlModel with
                        | null -> raise <| HttpException(404, "User not found.")
                        | x -> x |> toUserDetails
            }

        member __.getUserByName name =
            task {
                let! sqlModel = context.Users.SingleOrDefaultAsync(fun x -> name = x.Name) // Relies on case insensitivity of SQL 

                return match sqlModel with
                        | null -> raise <| HttpException(404, "User not found.")
                        | x -> x |> toUserDetails
            }

        member __.createUser request =
            task {
                let u = UserSqlModel()
                u.Name <- request.name
                u.Password <- request.password
                u.FailedLoginAttempts <- 0uy
                u.LastFailedLoginAttemptOn <- Nullable<DateTime>()
                u.CreatedOn <- DateTime.UtcNow

                try 
                    let! _ = context.Users.AddAsync(u)
                    let! _ = context.SaveChangesAsync()
                    return u |> toUserDetails
                with
                | :? InvalidOperationException as ex when ex.Message.StartsWith(nameConflictMessage) ->
                    return raise <| HttpException(409, "Conflict when attempting to write User.")
            }

        member __.deleteUser userId = 
            task {
                let! u = context.Users.FindAsync(userId)
                if u = null
                then return raise <| HttpException(404, "User not found.")
                else
                    context.Users.Remove(u) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return ()
            }

        member __.updateFailedLoginAttempts request =
            task {
                let! u = context.Users.FindAsync(request.userId)
                u.FailedLoginAttempts <- byte request.failedLoginAttempts
                u.LastFailedLoginAttemptOn <- request.lastFailedLoginAttemptOn |> Option.toNullable
                context.Users.Update(u) |> ignore
                let! _ = context.SaveChangesAsync()
                return ()
            }