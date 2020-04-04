namespace Apex.Api.Db.Mappings

open Apex.Api.Model
open System
open Apex.Api.Db.Model
open Apex.Api.Common.Json
open System.ComponentModel
open System.Collections.Generic
open Apex.Api.Enums

[<AutoOpen>]
module GameMappings =

    let toPlayer (source : PlayerSqlModel) : Player =
        let toIntOption (x : Nullable<byte>) : Option<int> =
            if x.HasValue then Some(int x.Value) else None

        {   
            id = source.PlayerId
            name = source.Name
            gameId = source.GameId
            userId = source.UserId |> Option.ofNullable
            kind = source.PlayerKindId
            status = source.PlayerStatusId
            colorId = source.ColorId |> toIntOption
            startingRegion = source.ColorId |>toIntOption
            startingTurnNumber = source.ColorId |> toIntOption
        }

    let toPlayerSqlModel (source : Player) : PlayerSqlModel =
        let toByteNullable (x : Option<int>) : Nullable<byte> =
            if x.IsSome then Nullable<byte>(byte x.Value) else Nullable<byte>()

        let x = PlayerSqlModel()
        x.PlayerId <- source.id
        x.GameId <- source.gameId
        x.UserId <- source.userId |> Option.toNullable
        x.PlayerKindId <- source.kind
        x.PlayerStatusId <- source.status
        x.Name <- source.name
        x.ColorId <- source.colorId |> toByteNullable
        x.StartingRegion <- source.startingRegion |> toByteNullable
        x.StartingTurnNumber <- source.startingTurnNumber |> toByteNullable
        x
        
    let createPlayerRequestToPlayerSqlModel (source : CreatePlayerRequest) : PlayerSqlModel =
        let x = PlayerSqlModel()
        x.UserId <- source.userId |> Option.toNullable
        x.PlayerKindId <- source.kind
        x.PlayerStatusId <- PlayerStatus.Pending
        x.Name <- source.name.Value
        x

    let toGame (source : GameSqlModel) : Game =
        {
            id = source.GameId
            createdBy = {
                userId = source.CreatedByUser.UserId
                userName = source.CreatedByUser.Name
                time = source.CreatedOn
            }
            parameters = {
                allowGuests = source.AllowGuests
                isPublic =source. IsPublic
                description = source.Description |> Option.ofObj
                regionCount = int source.RegionCount
            }
            status = source.GameStatusId
            players = source.Players |> Seq.map toPlayer |> Seq.toList
            pieces = source.PiecesJson |> JsonUtility.deserializeList
            turnCycle = source.TurnCycleJson |> JsonUtility.deserializeList
            currentTurn = source.CurrentTurnJson |> JsonUtility.deserializeOption
        }

    let toGameSqlModel (source : CreateGameRequest) : GameSqlModel =
        let x = GameSqlModel()
        x.IsPublic <- source.parameters.isPublic
        x.AllowGuests <- source.parameters.allowGuests
        x.Description <- source.parameters.description |> Option.toObj
        x.RegionCount <- byte source.parameters.regionCount
        x.Players <- List<PlayerSqlModel>()
        x.CreatedOn <- DateTime.UtcNow
        x.GameStatusId <- GameStatus.Pending
        x.CreatedByUserId <- source.createdByUserId
        x.CurrentTurnJson <- null
        x.TurnCycleJson <- null
        x.PiecesJson <- null
        x
    