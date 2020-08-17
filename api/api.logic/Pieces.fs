namespace Djambi.Api.Logic

open System
open System.Collections.Generic
open Djambi.Api.Model.GameModel
open Djambi.Api.Enums

type PieceStrategy() =
    abstract member moveMaxDistance : int
    abstract member canTargetWithMove : bool
    abstract member canTargetAfterMove : bool
    abstract member canTargetPiece : Piece -> Piece -> bool
    abstract member canStayInCenter : bool
    abstract member canEnterCenterToEvictPiece : bool
    abstract member canDropTarget : bool
    abstract member movesTargetToOrigin : bool
    abstract member killsTarget : bool
    abstract member killsControllingPlayerWhenKilled : bool
    abstract member isAlive : bool

    default x.moveMaxDistance = Int32.MaxValue
    default x.canTargetWithMove = false
    default x.canTargetAfterMove = false
    default x.canTargetPiece (subject : Piece) (target : Piece) = false
    default x.canStayInCenter = false
    default x.canEnterCenterToEvictPiece = false
    default x.canDropTarget = false
    default x.movesTargetToOrigin = false
    default x.killsTarget = false
    default x.killsControllingPlayerWhenKilled = false
    default x.isAlive = true

type HunterStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> PieceKind.Corpse &&
            target.playerId <> subject.playerId
        override x.canEnterCenterToEvictPiece = true
        override x.movesTargetToOrigin = true
        override x.killsTarget = true

type ConduitStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> PieceKind.Corpse &&
            target.playerId <> subject.playerId
        override x.canStayInCenter = true
        override x.canEnterCenterToEvictPiece = true
        override x.canDropTarget = true
        override x.killsTarget = true
        override x.killsControllingPlayerWhenKilled = true

type CorpseStrategy() =
    inherit PieceStrategy()
        override x.isAlive = false

type DiplomatStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> PieceKind.Corpse &&
            target.playerId <> subject.playerId
        override x.canEnterCenterToEvictPiece = true
        override x.canDropTarget = true

type ReaperStrategy() =
    inherit PieceStrategy() with
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind = PieceKind.Corpse
        override x.canEnterCenterToEvictPiece = true
        override x.canDropTarget = true

type ScientistStrategy() =
    inherit PieceStrategy() with
        override x.canTargetAfterMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> PieceKind.Corpse &&
            target.playerId <> subject.playerId
        override x.killsTarget = true

type ThugStrategy() =
    inherit PieceStrategy() with
        override x.moveMaxDistance = 2
        override x.canTargetWithMove = true
        override x.canTargetPiece (subject : Piece) (target : Piece) =
            target.kind <> PieceKind.Corpse &&
            target.playerId <> subject.playerId
        override x.canDropTarget = true
        override x.killsTarget = true

module Pieces =

    let private strategies = Dictionary<PieceKind, PieceStrategy>()
    strategies.Add(PieceKind.Hunter, HunterStrategy())
    strategies.Add(PieceKind.Conduit, ConduitStrategy())
    strategies.Add(PieceKind.Corpse, CorpseStrategy())
    strategies.Add(PieceKind.Diplomat, DiplomatStrategy())
    strategies.Add(PieceKind.Reaper, ReaperStrategy())
    strategies.Add(PieceKind.Scientist, ScientistStrategy())
    strategies.Add(PieceKind.Thug, ThugStrategy())

    let getStrategy (piece : Piece) : PieceStrategy =
        strategies.[piece.kind]

    let getStrategyForKind (kind : PieceKind) : PieceStrategy =
        strategies.[kind]