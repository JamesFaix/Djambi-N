namespace Apex.Api.Db.Model

open Apex.Api.Model

[<CLIMutable>]
type SnapshotJson = {
    game : Game
    history : Event list
}
