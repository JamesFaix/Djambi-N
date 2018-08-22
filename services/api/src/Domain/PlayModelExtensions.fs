namespace Djambi.Api.Domain

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Domain.BoardModels
open Djambi.Api.Domain.BoardsExtensions
open Djambi.Api.Common
open Djambi.Api.Common.Enums

open System.Linq

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

    type Selection with
        member this.isSubject =
            match this with
            | Subject _ -> true
            | _ -> false

        member this.isMove =
            match this with
            | Move _
            | MoveWithTarget (_,_) -> true
            | _ -> false

        member this.isTarget =
            match this with 
            | Target _
            | MoveWithTarget (_,_) -> true
            | _ -> false

        member this.isDrop =
            match this with
            | Drop _ -> true
            | _ -> false

        member this.maybePieceId =
            match this with
            | Subject id
            | Target id
            | MoveWithTarget (_,id) -> Some id
            | Move _
            | Drop _ -> None
    
        member this.maybeCellId =
            match this with
            | Move id 
            | Drop id
            | MoveWithTarget (id,_) -> Some id
            | Subject _ 
            | Target _ -> None

    type TurnState with 
        
        member this.getSubjectPieceId : int option = 
            this.selections 
            |> List.tryFind (fun s -> s.isSubject)
            |> Option.map (fun s -> s.maybePieceId.Value)
                        
        member this.getDestinationCellId : int option =
            this.selections
            |> List.tryFind (fun s -> s.isMove)
            |> Option.map (fun s -> s.maybeCellId.Value)
        
        member this.getTargetPieceId : int option =
            this.selections
            |> List.tryFind (fun s -> s.isTarget)
            |> Option.map (fun s -> s.maybePieceId.Value)

        member this.getDropCellId : int option =
            this.selections
            |> List.tryFind (fun s -> s.isDrop)
            |> Option.map (fun s -> s.maybeCellId.Value)

        member this.getVacateCellId : int option =
            let moves = this.selections |> List.filter (fun s -> s.isMove)
            match moves.Length with 
            | 2 -> Some(moves.[1].maybeCellId.Value)
            | _ -> None

        member private this.getPieceFromId(gameState : GameState, id : int) : Piece =
            gameState.pieces |> List.find (fun p -> p.id = id)
            
        member private this.getCellFromId(regionCount : int, id : int) : Cell =
            let board = BoardUtility.getBoardMetadata regionCount
            board.cell id

        member this.getSubjectPiece(gameState : GameState) : Piece option =
            this.getSubjectPieceId
            |> Option.map (fun id -> this.getPieceFromId(gameState, id))

        member this.getTargetPiece (gameState : GameState): Piece option =
            this.getTargetPieceId
            |> Option.map (fun id -> this.getPieceFromId(gameState, id))

        member this.getDestinationCell (regionCount : int) : Cell option =
            this.getDestinationCellId
            |> Option.map (fun id -> this.getCellFromId(regionCount, id))
            
    type GameState with 
        
        member this.currentPlayerId = 
            this.turnCycle.Head

        member this.piecesControlledBy playerId =
            this.pieces |> List.filter (fun p -> p.playerId = Some playerId)

        member this.piecesIndexedByCell =
            this.pieces |> Seq.map (fun p -> (p.cellId, p)) |> Map.ofSeq 

        member this.applyTurnState(turn : TurnState, regionCount : int) : Result<GameState, HttpError> =
            this.applyTurnStateToPieces(turn, regionCount)

            let turns = this.turnCycle.ToList()            

            //If subject is chief and destination is center, add extra turns to queue
            //If subject is chief and destination is not center, remove extra turns
            //If target is chief and subject is killer, remove extra turns
            //Update turn queue      
            //If next player has no moves, kill chief and abandon all pieces

            Ok(this)

        member private this.applyTurnStateToPieces(turn : TurnState, regionCount : int) : Result<GameState, HttpError> =
            let pieces = this.pieces.ToDictionary(fun p -> p.id)
        
            let result =
                match (turn.getSubjectPiece this, turn.getDestinationCell regionCount) with
                | (None, _) 
                | (_, None) -> Error({ statusCode = 500; message = "Cannot commit turn without subject selected." })
                | (Some subject, Some destination) -> 
                    let origin = subject.cellId
                    
                    //Move subject to destination or vacate cell
                    match turn.getVacateCellId with
                    | None ->        pieces.[subject.id] <- subject.moveTo destination.id
                    | Some vacate -> pieces.[subject.id] <- subject.moveTo vacate
                    
                    ////Add extra turns if chief is moving to center
                    //if subject.pieceType = Chief && destination.isCenter()
                    //then 

                    match turn.getTargetPiece this with 
                    | None -> ()
                    | Some target -> 
                        //Kill target 
                        if subject.isKiller 
                        then pieces.[target.id] <- target.kill

                        //Enlist players pieces if killing chief
                        if subject.isKiller && target.pieceType = Chief
                        then for p in this.piecesControlledBy target.playerId.Value do
                                pieces.[p.id] <- p.enlistBy subject.playerId.Value
                        
                        //Drop target if drop cell exists                        
                        match turn.getDropCellId with
                        | Some drop ->  pieces.[target.id] <- target.moveTo drop
                        | None -> ()
                        
                        //Move target back to origin if subject is assassin
                        if subject.pieceType = Assassin
                        then pieces.[target.id] <- subject.moveTo origin

                    Ok()

            result |> Result.map(fun _ -> { this with pieces = pieces.Values |> Seq.toList })

        