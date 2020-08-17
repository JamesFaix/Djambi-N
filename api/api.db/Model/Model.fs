namespace Djambi.Api.Db.Model

open Djambi.Api.Model

[<CLIMutable>]
type SnapshotJson = {
    game : Game
    history : Event list
}
