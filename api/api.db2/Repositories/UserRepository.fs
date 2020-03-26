namespace Apex.Api.Db.Repositories

open Apex.Api.Db.Interfaces
open System
open Apex.Api.Db.Model
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open Apex.Api.Db.Mappings
open Microsoft.EntityFrameworkCore

type UserRepository(context : ApexDbContext) =    
    interface IUserRepository with
        member __.getUser userId =
            task {
                let! sqlModel = context.Users.FindAsync(userId)

                return match sqlModel with
                        | null -> Error <| HttpException(404, "Not found.")
                        | x -> Ok(x |> toUserDetails)
            }

        member __.getUserByName name =
            task {
                let! sqlModel = context.Users.SingleOrDefaultAsync(fun x -> 
                    String.Equals(name, x.Name, StringComparison.InvariantCultureIgnoreCase))

                return match sqlModel with
                        | null -> Error <| HttpException(404, "Not found.")
                        | x -> Ok(x |> toUserDetails)
            }

        member __.createUser request =
            task {
                let u = UserSqlModel()
                u.Name <- request.name
                u.Password <- request.password
                u.FailedLoginAttempts <- 0uy
                u.LastFailedLoginAttemptOn <- Nullable<DateTime>()
                u.CreatedOn <- DateTime.UtcNow
                u.Privileges <- System.Collections.Generic.List<PrivilegeSqlModel>()

                let! _ = context.Users.AddAsync(u)
                let! _ = context.SaveChangesAsync()

                return u |> toUserDetails |> Ok
            }

        member __.deleteUser userId = 
            task {
                let! u = context.Users.FindAsync(userId)
                context.Users.Remove(u) |> ignore
                let! _ = context.SaveChangesAsync()
                return Ok ()
            }

        member __.updateFailedLoginAttempts request =
            task {
                let! u = context.Users.FindAsync(request.userId)
                u.FailedLoginAttempts <- byte request.failedLoginAttempts
                u.LastFailedLoginAttemptOn <- request.lastFailedLoginAttemptOn |> Option.toNullable
                context.Users.Update(u) |> ignore
                let! _ = context.SaveChangesAsync()
                return Ok ()
            }