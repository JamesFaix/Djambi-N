namespace Djambi.Api.Domain

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Domain.BoardModels
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Common
open Djambi.Api.Common.Enums

open System.Collections.Generic
module PlayModelExtensions =

    type Piece with
        member this.moveTo cellId =
            { this with cellId = cellId }

        member this.kill =
            { this with pieceType = Corpse; playerId = None }

        member this.enlistBy playerId =
            { this with playerId = Some playerId }

        member this.abandon =
            { this with playerId = None }

        member this.isKiller =
            match this.pieceType with
            | Chief | Thug | Reporter | Assassin -> true
            | _ -> false

        member this.isAlive =
            match this.pieceType with 
            | Corpse -> false
            | _ -> true

    type Player with
        member this.kill : Player =
            { this with isAlive = false }

    type TurnState with 

        member this.subject : Selection option =
            this.selections 
            |> List.tryFind (fun s -> s.selectionType = Subject)
                        
        member this.subjectPieceId : int option = 
            this.subject
            |> Option.map (fun s -> s.pieceId.Value)
                        
        member this.subjectCellId : int option =
            this.subject
            |> Option.map (fun s -> s.cellId)

        member this.destinationCellId : int option =
            this.selections
            |> List.tryFind (fun s -> s.selectionType = Move)
            |> Option.map (fun s -> s.cellId)
        
        member this.targetPieceId : int option =
            this.selections
            |> List.tryFind (fun s -> s.selectionType = Target || (s.selectionType = Move && s.pieceId.IsSome))
            |> Option.map (fun s -> s.pieceId.Value)

        member this.dropCellId : int option =
            this.selections
            |> List.tryFind (fun s -> s.selectionType = Drop)
            |> Option.map (fun s -> s.cellId)

        member this.vacateCellId : int option =
            let moves = this.selections |> List.filter (fun s -> s.selectionType = Move)
            match moves.Length with 
            | 2 -> Some(moves.[1].cellId)
            | _ -> None

        member private this.getPieceFromId(gameState : GameState)(id : int) : Piece =
            gameState.pieces |> List.find (fun p -> p.id = id)
            
        member private this.getCellFromId(regionCount : int)(id : int) : Cell =
            let board = BoardUtility.getBoardMetadata regionCount
            board.cell id

        member this.subjectPiece(gameState : GameState) : Piece option =
            this.subjectPieceId
            |> Option.map (this.getPieceFromId gameState)

        member this.targetPiece (gameState : GameState): Piece option =
            this.targetPieceId
            |> Option.map (this.getPieceFromId gameState)

        member this.subjectCell(regionCount : int) : Cell option =
            this.subjectCellId
            |> Option.map (this.getCellFromId regionCount)

        member this.destinationCell (regionCount : int) : Cell option =
            this.destinationCellId
            |> Option.map (this.getCellFromId regionCount)
            
        member this.dropCell (regionCount : int) : Cell option =
            this.dropCellId
            |> Option.map (this.getCellFromId regionCount)

    type GameState with 
        
        member this.currentPlayerId = 
            this.turnCycle.Head

        member this.piecesControlledBy playerId =
            this.pieces |> List.filter (fun p -> p.playerId = Some playerId)

        member this.piecesIndexedByCell =
            this.pieces |> Seq.map (fun p -> (p.cellId, p)) |> Map.ofSeq 
            