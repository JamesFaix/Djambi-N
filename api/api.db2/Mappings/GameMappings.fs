namespace Apex.Api.Db.Mappings

open Apex.Api.Model
open System
open Apex.Api.Db.Model

[<AutoOpen>]
module GameMappings =

    let toGame (source : GameSqlModel) (players : seq<PlayerSqlModel>) : Game =
        raise <| NotImplementedException()

    let toGameSqlModel (source : CreateGameRequest) : GameSqlModel =
        raise <| NotImplementedException()

    let toPlayerSqlModel (source : Player) : PlayerSqlModel =
        raise <| NotImplementedException()

    let createPlayerRequestToPlayerSqlModel (source : CreatePlayerRequest) : PlayerSqlModel =
        raise <| NotImplementedException()

    let toGameStatusSqlId (source : GameStatus) : byte =
        raise <| NotImplementedException()
        