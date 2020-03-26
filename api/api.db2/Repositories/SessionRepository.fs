namespace Apex.Api.Db.Repositories

open Apex.Api.Db.Interfaces
open Apex.Api.Db.Model
open System
open FSharp.Control.Tasks
open Apex.Api.Common.Control
open Apex.Api.Db.Mappings
open Microsoft.EntityFrameworkCore

type SessionRepository(context : ApexDbContext) =
    interface ISessionRepository with
        member __.getSession query =
            task {
                let! s = context.Sessions.SingleOrDefaultAsync(fun s ->
                    (query.sessionId.IsNone || s.Id = query.sessionId.Value) &&
                    (query.token.IsNone || s.Token = query.token.Value) &&
                    (query.userId.IsNone || s.User.Id = query.userId.Value)
                )

                if s = null
                then return Error <| HttpException(404, "Not found.")
                else return Ok(s |> toSession)
            }

        member __.createSession request =
            task {
                let! u = context.Users.FindAsync(request.userId)
                
                if u = null
                then return Error <| HttpException(404, "Not found.")
                else    
                    let s = SessionSqlModel()
                    s.User <- u
                    s.Token <- request.token
                    s.CreatedOn <- DateTime.UtcNow
                    s.ExpiresOn <- request.expiresOn

                    let! _ = context.Sessions.AddAsync(s)

                    return Ok(s |> toSession)
            }

        member __.renewSessionExpiration (sessionId, expiresOn) =
            task {
                let! s = context.Sessions.FindAsync(sessionId)
                if s = null
                then return Error <| HttpException(404, "Not found.")
                else 
                    s.ExpiresOn <- expiresOn
                    let! _ = context.SaveChangesAsync()
                    return Ok(s |> toSession)
            }

        member __.deleteSession (sessionId, token) =
            if sessionId.IsSome && token.IsSome
            then raise <| ArgumentException("Cannot use both sessionID and token to delete session.")

            if sessionId.IsNone && token.IsNone
            then raise <| ArgumentException("Delete session requires either a sessionID or a token to lookup the session.")

            task {
                let mutable s : SessionSqlModel = null

                if sessionId.IsSome
                then
                    let! x = context.Sessions.FindAsync(sessionId.Value)
                    s <- x

                if token.IsSome
                then 
                    let! x = context.Sessions.SingleOrDefaultAsync(fun s -> 
                        String.Equals(s.Token, token.Value, StringComparison.InvariantCulture))
                    s <- x

                if s = null
                then return Error <| HttpException(404, "Not found.")
                else
                    context.Sessions.Remove(s) |> ignore
                    let! _ = context.SaveChangesAsync()

                    return Ok ()
            }
