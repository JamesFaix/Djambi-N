namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db;
open Djambi.Api.Db.Interfaces
open Djambi.Api.Db.Model
open Djambi.Api.Model

type SessionRepository(u : SqlUtility,
                       userRepo : IUserRepository) =
    interface ISessionRepository with
        member x.getSession query =
            let cmd = Commands2.getSession query
            u.querySingle<SessionSqlModel>(cmd)
            |> thenBindAsync (fun sessionSqlModel -> 
                userRepo.getUser sessionSqlModel.userId
                |> thenMap (fun userDetails ->
                    let user = userDetails |> UserDetails.hideDetails
                    Mapping.mapSessionResponse sessionSqlModel user
                )
            )

        member x.createSession request =
            let cmd = Commands2.createSession request
            u.querySingle<int>(cmd)
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
            let cmd = Commands.renewSessionExpiration (sessionId, expiresOn)
            u.queryUnit(cmd)
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
            let cmd = Commands.deleteSession (sessionId, token)
            u.queryUnit(cmd)