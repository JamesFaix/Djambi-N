[<AutoOpen>]
module Apex.Api.Model.SnapshotModel

open Apex.ClientGenerator.Annotations

type Snapshot =
    {
        id : int
        createdBy : CreationSource
        description : string
        game : Game
        history : Event list
    }

[<ClientType(ClientSection.Snapshots)>]
type SnapshotInfo =
    {
        id : int
        createdBy : CreationSource
        description : string
    }

[<ClientType(ClientSection.Snapshots)>]
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