namespace Apex.Api.Db.Mappings

open Apex.Api.Model
open System
open Apex.Api.Db.Model
open Apex.Api.Common.Json
open System.ComponentModel
open System.Collections.Generic

[<AutoOpen>]
module GameMappings =

    let toGameStatusSqlId (source : GameStatus) : byte =
        match source with
        | GameStatus.Canceled -> 1uy
        | GameStatus.InProgress -> 2uy
        | GameStatus.Over -> 3uy
        | GameStatus.Pending -> 4uy

    let toGameStatus (source : GameStatusSqlModel) : GameStatus =
        match source.Id with
        | 1uy -> GameStatus.Canceled
        | 2uy -> GameStatus.InProgress
        | 3uy -> GameStatus.Over
        | 4uy -> GameStatus.Pending
        | _ -> raise <| InvalidEnumArgumentException()

    let toPlayer (source : PlayerSqlModel) : Player =
        raise <| NotImplementedException()

    let toGame (source : GameSqlModel) (players : seq<PlayerSqlModel>) : Game =
        {
            id = source.Id
            createdBy = {
                userId = source.CreatedByUser.Id
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            parameters = {
                allowGuests = source.AllowGuests
                isPublic =source. IsPublic
                description = source.Description |> Option.ofObj
                regionCount = int source.RegionCount
            }
            status = source.Status |> toGameStatus
            players = players |> Seq.map toPlayer |> Seq.toList
            pieces = source.PiecesJson |> JsonUtility.deserialize
            turnCycle = source.TurnCycleJson |> JsonUtility.deserialize
            currentTurn = source.CurrentTurnJson |> JsonUtility.deserialize
        }

    let toGameSqlModel (source : CreateGameRequest) (user : UserSqlModel) (status : GameStatusSqlModel) : GameSqlModel =
        let x = GameSqlModel()
        x.IsPublic <- source.parameters.isPublic
        x.AllowGuests <- source.parameters.allowGuests
        x.Description <- source.parameters.description |> Option.toObj
        x.RegionCount <- byte source.parameters.regionCount
        x.Players <- List<PlayerSqlModel>()
        x.CreatedOn <- DateTime.UtcNow
        x.Status <- status
        x.CreatedByUser <- user
        x.CurrentTurnJson <- null
        x.TurnCycleJson <- null
        x.PiecesJson <- null
        x

    let toPlayerSqlModel (source : Player) : PlayerSqlModel =
        raise <| NotImplementedException()

    let createPlayerRequestToPlayerSqlModel (source : CreatePlayerRequest) : PlayerSqlModel =
        raise <| NotImplementedException()

        