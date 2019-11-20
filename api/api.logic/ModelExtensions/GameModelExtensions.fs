namespace Apex.Api.Logic.ModelExtensions

open Apex.Api.Logic.ModelExtensions.BoardModelExtensions
open Apex.Api.Model

module GameModelExtensions =

    type Piece with
        member this.moveTo cellId =
            { this with cellId = cellId }

        member this.kill =
            { this with kind = Corpse; playerId = None }

        member this.enlistBy playerId =
            { this with playerId = Some playerId }

        member this.abandon =
            { this with playerId = None }

    type Turn with

        member this.subject : Selection option =
            this.selections
            |> List.tryFind (fun s -> s.kind = Subject)

        member this.subjectPieceId : int option =
            this.subject
            |> Option.map (fun s -> s.pieceId.Value)

        member this.subjectCellId : int option =
            this.subject
            |> Option.map (fun s -> s.cellId)

        member this.destinationCellId : int option =
            this.selections
            |> List.tryFind (fun s -> s.kind = Move)
            |> Option.map (fun s -> s.cellId)

        member this.targetPieceId : int option =
            this.selections
            |> List.tryFind (fun s -> s.kind = Target || (s.kind = Move && s.pieceId.IsSome))
            |> Option.map (fun s -> s.pieceId.Value)

        member this.dropCellId : int option =
            this.selections
            |> List.tryFind (fun s -> s.kind = Drop)
            |> Option.map (fun s -> s.cellId)

        member this.vacateCellId : int option =
            let moves = this.selections |> List.filter (fun s -> s.kind = Move)
            match moves.Length with
            | 2 -> Some(moves.[1].cellId)
            | _ -> None

        member private this.getPieceFromId(game : Game)(id : int) : Piece =
            game.pieces |> List.find (fun p -> p.id = id)

        member private this.getCellFromId(regionCount : int)(id : int) : Cell option =
            let board = BoardModelUtility.getBoardMetadata regionCount
            board.cell id

        member this.subjectPiece(game : Game) : Piece option =
            this.subjectPieceId
            |> Option.map (this.getPieceFromId game)

        member this.targetPiece (game : Game): Piece option =
            this.targetPieceId
            |> Option.map (this.getPieceFromId game)

        member this.subjectCell(regionCount : int) : Cell option =
            this.subjectCellId
            |> Option.bind (this.getCellFromId regionCount)

        member this.destinationCell (regionCount : int) : Cell option =
            this.destinationCellId
            |> Option.bind (this.getCellFromId regionCount)

        member this.dropCell (regionCount : int) : Cell option =
            this.dropCellId
            |> Option.bind (this.getCellFromId regionCount)

    type Game with

        member this.currentPlayerId =
            this.turnCycle.Head

        member this.piecesControlledBy playerId =
            this.pieces |> List.filter (fun p -> p.playerId = Some playerId)

        member this.piecesIndexedByCell =
            this.pieces |> Seq.map (fun p -> (p.cellId, p)) |> Map.ofSeq