namespace Djambi.Api.Logic.PieceStrategies

open System
open System.Collections.Generic
open Djambi.Api.Model.GameModel

type PieceStrategy() =
    abstract member moveMaxDistance : int
    abstract member moveCanTargetPiece : Piece -> Piece -> bool
    abstract member canStayInSeat : bool
    abstract member moveCanEnterSeatToEvictPiece : Piece -> Piece -> bool
    
    default x.moveMaxDistance = Int32.MaxValue
    default x.moveCanTargetPiece (subject : Piece) (target : Piece) = false
    default x.canStayInSeat = false
    default x.moveCanEnterSeatToEvictPiece (subject : Piece) (target : Piece) = false

type AssassinStrategy() =
    inherit PieceStrategy() with
        override x.moveCanTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId
        override x.moveCanEnterSeatToEvictPiece (subject : Piece) (target : Piece) =
            target.kind = Chief &&
            target.playerId <> subject.playerId

type ChiefStrategy() =
    inherit PieceStrategy() with
        override x.moveCanTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId
        override x.canStayInSeat = true
        override x.moveCanEnterSeatToEvictPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId

type CorpseStrategy() =
    inherit PieceStrategy()

type DiplomatStrategy() =
    inherit PieceStrategy() with
        override x.moveCanTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId = subject.playerId
        override x.moveCanEnterSeatToEvictPiece (subject : Piece) (target : Piece) =
            target.kind = Chief &&
            target.playerId <> subject.playerId
            
type GravediggerStrategy() =
    inherit PieceStrategy() with
        override x.moveCanTargetPiece (subject : Piece) (target : Piece) =
            target.kind = Corpse
        override x.moveCanEnterSeatToEvictPiece (subject : Piece) (target : Piece) =
            target.kind = Corpse

type ReporterStrategy() =
    inherit PieceStrategy()

type ThugStrategy() =
    inherit PieceStrategy() with
        override x.moveMaxDistance = 2       
        override x.moveCanTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> Corpse &&
            target.playerId <> subject.playerId

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