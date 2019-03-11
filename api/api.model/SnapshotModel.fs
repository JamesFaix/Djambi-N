[<AutoOpen>]
module Djambi.Api.Model.SnapshotModel

open System
open Djambi.ClientGenerator.Annotations

type Snapshot = 
    {
        id : int
        createdByUserId : int
        createdOn : DateTime
        description : string
        game : Game
        history : Event list
    }
    
[<ClientType(ClientSection.Misc)>]
type SnapshotInfo =
    {
        id : int
        createdByUserId : int
        createdOn : DateTime
        description : string    
    }

type CreateSnapshotRequest =
    {
        createdByUserId : int
        description : string    
        game : Game
        history : Event list
    }