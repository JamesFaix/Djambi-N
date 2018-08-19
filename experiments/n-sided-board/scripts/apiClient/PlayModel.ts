export class Player {
    readonly id : number
    readonly userId : number
    readonly name : string
    readonly isAlive : boolean
}

export enum PieceType {
    Chief = "Chief",
    Assassin = "Assassin",
    Diplomat = "Diplomat",
    Reporter = "Reporter",
    Thug = "Thug",
    Gravedigger = "Gravedigger"
}

export class Piece {
    readonly id : number
    readonly type : PieceType
    readonly playerId : number
    readonly originalPlayerId : number
    readonly isAlive : boolean
    readonly cellId : number
}

export class GameState {
    readonly players : Array<Player>
    readonly pieces : Array<Piece>
    readonly turnCycle : Array<number>
}

export class PlayerStartConditions {
    readonly playerId : number
    readonly color : number
    readonly region : number
    readonly turnNumber : number
}

export enum TurnStatus {
    AwaitingSelection = "AwaitingSelection",
    AwaitingConfirmation = "AwaitingConfirmation"
}

export enum SelectionType {
    Subject = "Subject",
    Move = "Move",
    MoveWithTarget = "MoveWithTarget",
    Target = "Target",
    Drop = "Drop"
}

export class Selection {
    readonly type : SelectionType
    readonly cellId : number
    readonly pieceId : number
}

export class TurnState {
    readonly status : TurnStatus
    readonly selections : Array<Selection>
    readonly selectionOptions : Array<number>
}

export class GameStartResponse {
    readonly gameState : GameState
    readonly startingConditions : Array<PlayerStartConditions>
    readonly turnState : TurnState
}