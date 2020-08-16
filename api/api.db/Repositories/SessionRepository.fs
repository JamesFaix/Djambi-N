namespace Djambi.Api.Db.Repositories

open Djambi.Api.Db.Interfaces
open Djambi.Api.Db.Model
open System
open FSharp.Control.Tasks
open Djambi.Api.Db.Mappings
open Microsoft.EntityFrameworkCore
open System.Security.Authentication

type SessionRepository(context : DjambiDbContext) =
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

                return s |> Option.ofObj |> Option.map toSession
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

                return s |> toSession
            }

        member __.renewSessionExpiration (sessionId, expiresOn) =
            task {
                let! s = context.Sessions.FindAsync(sessionId)
                if s = null
                then return raise <| AuthenticationException("Not signed in.")
                else 
                    s.ExpiresOn <- expiresOn
                    context.Sessions.Update(s) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return s |> toSession
            }

        member __.deleteSession token =
            task {
                let! session = context.Sessions.SingleOrDefaultAsync(fun s -> s.Token = token)
                if session = null
                then return ()
                else
                    context.Sessions.Remove(session) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return ()
            }
