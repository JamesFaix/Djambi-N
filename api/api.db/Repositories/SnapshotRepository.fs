namespace Djambi.Api.Db.Repositories

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db
open Djambi.Api.Db.Interfaces

type SnapshotRepository(ctxProvider : CommandContextProvider) =

    interface ISnapshotRepository with
        member x.getSnapshot snapshotId =
            Commands2.getSnapshot snapshotId
            |> Command.execute ctxProvider
            |> thenMap Mapping.mapSnapshotFromSql

        member x.getSnapshotsForGame gameId =
            Commands2.getSnapshots gameId
            |> Command.execute ctxProvider
            |> thenMap (List.map Mapping.mapSnapshotInfoFromSql)

        member x.deleteSnapshot snapshotId =
            Commands.deleteSnapshot snapshotId
            |> Command.execute ctxProvider

        member x.createSnapshot request =
            Commands2.createSnapshot request
            |> Command.execute ctxProvider

        member x.loadSnapshot (gameId, snapshotId) =            
            (x :> ISnapshotRepository).getSnapshot snapshotId
            |> thenBindAsync (fun snapshot ->
                let commands = 
                    Commands2.replaceEventHistory (gameId, snapshot.history) ::
                    Commands2.updateGame snapshot.game ::
                    (snapshot.game.players |> List.map Commands2.updatePlayer)

                SqlUtility.executeTransactionally commands (Some "Snapshot") ctxProvider
            )