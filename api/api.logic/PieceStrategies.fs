namespace Djambi.Api.Logic.PieceStrategies

open System
open System.Collections.Generic
open Djambi.Api.Model.GameModel

type PieceStrategy() =
    abstract member moveMaxDistance : int
    abstract member canTargetWithMove : bool
    abstract member canTargetAfterMove : bool
    abstract member canTargetPiece : Piece -> Piece -> bool
    abstract member canStayInCenter : bool
    abstract member canEnterSeatToEvictPiece : bool
    abstract member canDropTarget : bool
    abstract member movesTargetToOrigin : bool
    
    default x.moveMaxDistance = Int32.MaxValue
    default x.canTargetWithMove = false
    default x.canTargetAfterMove = false
    default x.canTargetPiece (subject : Piece) (target : Piece) = false
    default x.canStayInCenter = false
    default x.canEnterSeatToEvictPiece = false
    default x.canDropTarget = false
    default x.movesTargetToOrigin = false

type AssassinStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId
        override x.canEnterSeatToEvictPiece = true
        override x.movesTargetToOrigin = true

type ChiefStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId
        override x.canStayInCenter = true
        override x.canEnterSeatToEvictPiece = true
        override x.canDropTarget = true

type CorpseStrategy() =
    inherit PieceStrategy()

type DiplomatStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId = subject.playerId
        override x.canEnterSeatToEvictPiece = true
        override x.canDropTarget = true
            
type GravediggerStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind = Corpse
        override x.canEnterSeatToEvictPiece = true
        override x.canDropTarget = true

type ReporterStrategy() =
    inherit PieceStrategy() with
        override x.canTargetAfterMove = true        
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId

type ThugStrategy() =
    inherit PieceStrategy() with
        override x.moveMaxDistance = 2       
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId
        override x.canDropTarget = true

module PieceService =

    let private strategies = Dictionary<PieceKind, PieceStrategy>()
    strategies.Add(Assassin, AssassinStrategy())
    strategies.Add(Chief, ChiefStrategy())
    strategies.Add(Corpse, CorpseStrategy())
    strategies.Add(Diplomat, DiplomatStrategy())
    strategies.Add(Gravedigger, GravediggerStrategy())
    strategies.Add(Reporter, ReporterStrategy())
    strategies.Add(Thug, ThugStrategy())

    let getStrategy (piece : Piece) : PieceStrategy =
        strategies.[piece.kind]