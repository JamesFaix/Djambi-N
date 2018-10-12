module Djambi.Api.Common.Enums

type GameStatus =
    | Open
    | Started
    | Complete
    | Cancelled
        
type TurnStatus =
    | AwaitingSelection
    | AwaitingConfirmation

type PieceType =
    | Chief
    | Thug
    | Reporter
    | Assassin
    | Diplomat
    | Gravedigger
    | Corpse