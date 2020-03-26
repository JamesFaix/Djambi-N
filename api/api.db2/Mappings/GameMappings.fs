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

    let toGameStatus (source : byte) : GameStatus =
        match source with
        | 1uy -> GameStatus.Canceled
        | 2uy -> GameStatus.InProgress
        | 3uy -> GameStatus.Over
        | 4uy -> GameStatus.Pending
        | _ -> raise <| InvalidEnumArgumentException()

    let toPlayerKindSqlId (source : PlayerKind) : byte =
        match source with
        | PlayerKind.User -> 1uy
        | PlayerKind.Guest -> 2uy
        | PlayerKind.Neutral -> 3uy

    let toPlayerKind (source : byte) : PlayerKind =
        match source with
        | 1uy -> PlayerKind.User
        | 2uy -> PlayerKind.Guest
        | 3uy -> PlayerKind.Neutral
        | _ -> raise <| InvalidEnumArgumentException()

    let toPlayerStatusSqlId (source : PlayerStatus) : byte =
        match source with
        | PlayerStatus.Pending -> 1uy
        | PlayerStatus.Alive -> 2uy
        | PlayerStatus.Eliminated -> 3uy
        | PlayerStatus.Conceded -> 4uy
        | PlayerStatus.WillConcede -> 5uy
        | PlayerStatus.AcceptsDraw -> 6uy
        | PlayerStatus.Victorious -> 7uy

    let toPlayerStatus (source : byte) : PlayerStatus =
        match source with
        | 1uy -> PlayerStatus.Pending
        | 2uy -> PlayerStatus.Alive
        | 3uy -> PlayerStatus.Eliminated
        | 4uy -> PlayerStatus.Conceded
        | 5uy -> PlayerStatus.WillConcede
        | 6uy -> PlayerStatus.AcceptsDraw
        | 7uy -> PlayerStatus.Victorious
        | _ -> raise <| InvalidEnumArgumentException()

    let toPlayer (source : PlayerSqlModel) : Player =
        let toIntOption (x : Nullable<byte>) : Option<int> =
            if x.HasValue then Some(int x.Value) else None

        {   
            id = source.Id
            name = source.Name
            gameId = source.GameId
            userId = source.UserId |> Option.ofNullable
            kind = source.KindId |> toPlayerKind
            status = source.StatusId |> toPlayerStatus
            colorId = source.ColorId |> toIntOption
            startingRegion = source.ColorId |>toIntOption
            startingTurnNumber = source.ColorId |> toIntOption
        }

    let toPlayerSqlModel (source : Player) : PlayerSqlModel =
        let toByteNullable (x : Option<int>) : Nullable<byte> =
            if x.IsSome then Nullable<byte>(byte x.Value) else Nullable<byte>()

        let x = PlayerSqlModel()
        x.Id <- source.id
        x.GameId <- source.gameId
        x.UserId <- source.userId |> Option.toNullable
        x.KindId <- source.kind |> toPlayerKindSqlId
        x.StatusId <- source.status |> toPlayerStatusSqlId
        x.Name <- source.name
        x.ColorId <- source.colorId |> toByteNullable
        x.StartingRegion <- source.startingRegion |> toByteNullable
        x.StartingTurnNumber <- source.startingTurnNumber |> toByteNullable
        x
        
    let createPlayerRequestToPlayerSqlModel (source : CreatePlayerRequest) (name : Option<string>) : PlayerSqlModel =
        let name = if source.name.IsSome then source.name.Value else name.Value

        let x = PlayerSqlModel()
        x.UserId <- source.userId |> Option.toNullable
        x.KindId <- source.kind |> toPlayerKindSqlId
        x.StatusId <- PlayerStatus.Pending |> toPlayerStatusSqlId
        x.Name <- name
        x

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
            status = source.StatusId |> toGameStatus
            players = players |> Seq.map toPlayer |> Seq.toList
            pieces = source.PiecesJson |> JsonUtility.deserialize
            turnCycle = source.TurnCycleJson |> JsonUtility.deserialize
            currentTurn = source.CurrentTurnJson |> JsonUtility.deserialize
        }

    let toGameSqlModel (source : CreateGameRequest) : GameSqlModel =
        let x = GameSqlModel()
        x.IsPublic <- source.parameters.isPublic
        x.AllowGuests <- source.parameters.allowGuests
        x.Description <- source.parameters.description |> Option.toObj
        x.RegionCount <- byte source.parameters.regionCount
        x.Players <- List<PlayerSqlModel>()
        x.CreatedOn <- DateTime.UtcNow
        x.StatusId <- GameStatus.Pending |> toGameStatusSqlId
        x.CreatedByUserId <- source.createdByUserId
        x.CurrentTurnJson <- null
        x.TurnCycleJson <- null
        x.PiecesJson <- null
        x
    