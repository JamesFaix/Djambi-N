namespace Djambi.Api.Db.Repositories

open Dapper
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Mapping
open Djambi.Api.Db.Model
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model
open Djambi.Api.Db.Interfaces

type SessionRepository(userRepo : IUserRepository) =
    interface ISessionRepository with
        member x.getSession query =
            let param = DynamicParameters()
                            .addOption("SessionId", query.sessionId)
                            .addOption("Token", query.token)
                            .addOption("UserId", query.userId)
            let cmd = proc("Sessions_Get", param)

            querySingle<SessionSqlModel>(cmd, "Session")
            |> thenBindAsync (fun sessionSqlModel -> 
                userRepo.getUser sessionSqlModel.userId
                |> thenMap (fun userDetails ->
                    let user = userDetails |> UserDetails.hideDetails
                    mapSessionResponse sessionSqlModel user
                )
            )

        member x.createSession request =
            let param = DynamicParameters()
                            .add("UserId", request.userId)
                            .add("Token", request.token)
                            .add("ExpiresOn", request.expiresOn)
            let cmd = proc("Sessions_Create", param)

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
            let param = DynamicParameters()
                            .add("SessionId", sessionId)
                            .add("ExpiresOn", expiresOn)
            let cmd = proc("Sessions_Renew", param)

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
            let param = DynamicParameters()
                            .addOption("SessionId", sessionId)
                            .addOption("Token", token)
            let cmd = proc("Sessions_Delete", param)

            queryUnit(cmd, "Session")