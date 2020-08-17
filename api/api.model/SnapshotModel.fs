[<AutoOpen>]
module Djambi.Api.Model.SnapshotModel

type Snapshot =
    {
        id : int
        createdBy : CreationSource
        description : string
        game : Game
        history : Event list
    }

type SnapshotInfo =
    {
        id : int
        createdBy : CreationSource
        description : string
    }

type CreateSnapshotRequest =
    {
        description : string
    }

type InternalCreateSnapshotRequest =
    {
        game : Game
        history : Event list
        createdByUserId : int
        description : string
    }

module Snapshot =
    let hideDetails (snapshot : Snapshot) : SnapshotInfo =
        {
            id = snapshot.id
            createdBy = snapshot.createdBy
            description = snapshot.description
        }