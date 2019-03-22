namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces

type SnapshotRepository(ctxProvider : CommandContextProvider) =

    interface ISnapshotRepository with
        member x.getSnapshot snapshotId =
            let cmd = Commands2.getSnapshot snapshotId
            (cmd.execute ctxProvider)
            |> thenMap Mapping.mapSnapshotFromSql

        member x.getSnapshotsForGame gameId =
            let cmd = Commands2.getSnapshots gameId
            (cmd.execute ctxProvider)
            |> thenMap (List.map Mapping.mapSnapshotInfoFromSql)

        member x.deleteSnapshot snapshotId =
            let cmd = Commands.deleteSnapshot snapshotId
            (cmd.execute ctxProvider)

        member x.createSnapshot request =
            let cmd = Commands2.createSnapshot request
            (cmd.execute ctxProvider)

        member x.loadSnapshot (gameId, snapshotId) =            
            (x :> ISnapshotRepository).getSnapshot snapshotId
            |> thenBindAsync (fun snapshot ->
                let commands = 
                    Commands2.replaceEventHistory (gameId, snapshot.history) ::
                    Commands2.updateGame snapshot.game ::
                    (snapshot.game.players |> List.map Commands2.updatePlayer)

                SqlUtility.executeTransactionally commands (Some "Snapshot") ctxProvider
            )