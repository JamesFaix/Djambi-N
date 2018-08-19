namespace Djambi.Api.Domain

open Djambi.Api.Domain.PlayModels
open Djambi.Api.Domain.BoardModels
open Djambi.Api.Domain.BoardsExtensions

module PlayModelExtensions =

    type GameState with 
        
        member this.currentPlayerId = 
            this.turnCycle.Head

        member this.piecesControlledBy playerId =
            this.pieces |> List.filter (fun p -> p.playerId = Some playerId)

        member this.piecesIndexedByCell =
            this.pieces |> Seq.map (fun p -> (p.cellId, p)) |> Map.ofSeq 

    type TurnState with 
        
        member this.getSubjectPieceId : int option = 
            this.selections 
            |> List.tryFind (fun s -> match s with | Subject _ -> true | _ -> false)
            |> Option.bind (fun s -> match s with | Subject pieceId -> Some pieceId | _ -> None)

        member this.getSubjectPiece(gameState : GameState) : Piece option =
            match this.getSubjectPieceId with
            | None -> None
            | Some id -> Some (gameState.pieces |> List.find (fun p -> p.id = id))
    
        member this.getDestinationCellId : int option =
            this.selections
            |> List.tryFind (fun s -> 
                                match s with 
                                | Move _ -> true 
                                | MoveWithTarget (_,_) -> true 
                                | _ -> false)
            |> Option.bind (fun s -> 
                                match s with 
                                | Move cellId -> Some cellId 
                                | MoveWithTarget (cellId, _) -> Some cellId 
                                | _ -> None)

        member this.getDestinationCell (boardRegionCount : int) : Cell option =
            match this.getDestinationCellId with
                | None -> None
                | Some id -> 
                    let board = BoardUtility.getBoardMetadata boardRegionCount
                    Some (board.cells() |> Seq.find (fun c -> c.id = id))