namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces
open Djambi.Api.Db;

type SessionRepository(userRepo : IUserRepository) =
    interface ISessionRepository with
        member x.getSession query =
            let cmd = Commands2.getSession query
            querySingle<SessionSqlModel>(cmd, "Session")
            |> thenBindAsync (fun sessionSqlModel -> 
                userRepo.getUser sessionSqlModel.userId
                |> thenMap (fun userDetails ->
                    let user = userDetails |> UserDetails.hideDetails
                    Mapping.mapSessionResponse sessionSqlModel user
                )
            )

        member x.createSession request =
            let cmd = Commands2.createSession request
            querySingle<int>(cmd, "Session")
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
            queryUnit(cmd, "Session")
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
            queryUnit(cmd, "Session")