namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db;
open Djambi.Api.Db.Interfaces
open Djambi.Api.Model

type SessionRepository(ctxProvider : CommandContextProvider,
                       userRepo : IUserRepository) =
    interface ISessionRepository with
        member x.getSession query =
            Commands2.getSession query
            |> Command.execute ctxProvider
            |> thenBindAsync (fun sessionSqlModel -> 
                userRepo.getUser sessionSqlModel.userId
                |> thenMap (fun userDetails ->
                    let user = userDetails |> UserDetails.hideDetails
                    Mapping.mapSessionResponse sessionSqlModel user
                )
            )

        member x.createSession request =
            Commands2.createSession request
            |> Command.execute ctxProvider
            |> thenBindAsync(fun sessionId -> 
                let query = 
                    {
                        sessionId = Some sessionId
                        token = None
                        userId = None
                    }    
                (x :> ISessionRepository).getSession query
            )

        member x.renewSessionExpiration (sessionId, expiresOn) =
            Commands.renewSessionExpiration (sessionId, expiresOn)
            |> Command.execute ctxProvider
            |> thenBindAsync (fun _ -> 
                let query = 
                    {
                        sessionId = Some sessionId
                        token = None
                        userId = None
                    }    
                (x :> ISessionRepository).getSession query
            )

        member x.deleteSession (sessionId, token) =
            Commands.deleteSession (sessionId, token)
            |> Command.execute ctxProvider