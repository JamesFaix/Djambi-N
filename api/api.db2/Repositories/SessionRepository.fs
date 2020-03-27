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
                let! s = 
                    context.Sessions
                        .Include(fun s -> s.User)
                        .SingleOrDefaultAsync(fun s ->
                            (query.sessionId.IsNone || s.SessionId = query.sessionId.Value) &&
                            (query.token.IsNone || s.Token = query.token.Value) &&
                            (query.userId.IsNone || s.User.UserId = query.userId.Value)
                        )

                if s = null
                then return Error <| HttpException(404, "Not found.")
                else return Ok(s |> toSession)
            }

        member __.createSession request =
            task {
                let s = SessionSqlModel()
                s.UserId <- request.userId
                s.Token <- request.token
                s.CreatedOn <- DateTime.UtcNow
                s.ExpiresOn <- request.expiresOn

                let! _ = context.Sessions.AddAsync(s)
                let! _ = context.SaveChangesAsync()

                let! s = 
                    context.Sessions
                        .Include(fun s -> s.User)
                        .SingleOrDefaultAsync(fun s1 -> s1.SessionId = s.SessionId)

                return Ok(s |> toSession)
            }

        member __.renewSessionExpiration (sessionId, expiresOn) =
            task {
                let! s = context.Sessions.FindAsync(sessionId)
                if s = null
                then return Error <| HttpException(404, "Not found.")
                else 
                    s.ExpiresOn <- expiresOn
                    context.Sessions.Update(s) |> ignore
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
