import * as Sprintf from 'sprintf-js';
import BoardService from './boardService';
import Debug from './debug';
import ThemeService from './themes/themeService';
import {
    Effect,
    EffectKind,
    Event,
    EventKind,
    Game,
    GameStatusChangedEffect,
    Location,
    NeutralPlayerAddedEffect,
    Piece,
    PieceAbandonedEffect,
    PieceDroppedEffect,
    PieceEnlistedEffect,
    PieceKilledEffect,
    PieceKind,
    PieceMovedEffect,
    PieceVacatedEffect,
    Player,
    PlayerAddedEffect,
    PlayerKind,
    PlayerOutOfMovesEffect,
    PlayerRemovedEffect,
    PlayerStatusChangedEffect,
    Selection,
    SelectionKind,
    TurnCycleAdvancedEffect,
    TurnCyclePlayerFellFromPowerEffect,
    TurnCyclePlayerRemovedEffect,
    TurnCyclePlayerRoseToPowerEffect
} from './api/model';

export default class CopyService {
    constructor(
        private readonly boards : BoardService,
        private readonly theme : ThemeService
    ){

    }

    public locationToString(location : Location) : string {
        return `(${location.region}, ${location.x}, ${location.y})`;
    }

    public getCellLabel(cellId : number, regionCount : number) : string {
        //The first time the board is needed it must be fetched from the API.
        //To avoid making this asynchronous, this just skips the API call if the board is not already cached.
        const board = this.boards.getBoardIfCached(regionCount);
        if (board) {
            const cell = board.cells.find(c => c.id === cellId);

            const base = cell.locations.find(l => l.x === 0 && l.y === 0)
                ? this.theme.getCenterCellName()
                : this.locationToString(cell.locations[0]);

            if (Debug.showPieceAndCellIds){
                return Sprintf.sprintf("%s (#%i)", base, cellId);
            } else {
                return base;
            }
        } else {
            return cellId.toString();
        }
    }

    public getPieceLabel(piece : Piece, game : Game) : string {
        const kindName = this.theme.getPieceName(piece.kind);
        if (piece.kind === PieceKind.Corpse) {
            return kindName;
        }

        const player = game.players.find(p => p.id === piece.playerId);
        const base = player
            ? `${player.name}'s ${kindName}`
            : `Neutral ${kindName}`;

        if (Debug.showPieceAndCellIds){
            return `${base} (#${piece.id})`;
        } else {
            return base;
        }
    }

    public getSelectionDescription(selection : Selection, game : Game) : string {
        const format = this.theme.getSelectionDescriptionTemplate(selection);

        const cell = selection.cellId
            ? this.getCellLabel(selection.cellId, game.parameters.regionCount)
            : null;

        const piece = selection.pieceId
            ? this.getPieceLabel(game.pieces.find(p => p.id === selection.pieceId), game)
            : null;

        switch (selection.kind) {
            case SelectionKind.Drop:
                return Sprintf.sprintf(format, cell);

            case SelectionKind.Move:
                if (piece === null) {
                    return Sprintf.sprintf(format, cell);
                } else {
                    return Sprintf.sprintf(format, cell, piece);
                }

            case SelectionKind.Subject:
                return Sprintf.sprintf(format, piece);

            case SelectionKind.Target:
                return Sprintf.sprintf(format, piece, cell);

            case SelectionKind.Vacate:
                return Sprintf.sprintf(format, this.theme.getCenterCellName(), cell);

            default: throw "Invalid selection kind.";
        }
    }

    //--- Players ---

    public getPlayerLabel(playerId : number, game : Game) : string {
        return game.players.find(p => p.id === playerId).name;
    }

    public getPlayerNote(player : Player, game : Game) {
        switch (player.kind) {
            case PlayerKind.User:
                return "";

            case PlayerKind.Guest:
                const host = game.players
                    .find(p => p.userId === player.userId
                        && p.kind === PlayerKind.User);

                return "Guest of " + host.name;

            case PlayerKind.Neutral:
                return "Neutral";

            default:
                throw "Invalid player kind.";
        }
    }

    //--- Events ---

    public getEventMessage(game : Game, event : Event) : string {
        const actingPlayer = game.players.find(p => p.id === event.actingPlayerId);
        const actingPlayerName = actingPlayer ? actingPlayer.name : "[Admin]";

        const template = this.theme.getEventMessageTemplate(event);

        switch (event.kind) {
            case EventKind.GameStarted:
                return template;

            case EventKind.TurnCommitted:
                return Sprintf.sprintf(template, {
                    player: actingPlayerName
                });

            case EventKind.PlayerStatusChanged:
                return Sprintf.sprintf(template, {
                    player: actingPlayerName
                });

            default:
                throw "Unsupported event kind.";
        }
    }

    //--- Effects ---

    public getEffectMessage(game : Game, effect : Effect) : string {
        switch (effect.kind) {
            case EffectKind.GameStatusChanged:
                return this.getGameStatusChangedMessage(game, effect);
            case EffectKind.NeutralPlayerAdded:
                return this.getNeutralPlayerAddedMessage(game, effect);
            case EffectKind.PieceAbandoned:
                return this.getPieceAbandonedMessage(game, effect);
            case EffectKind.PieceDropped:
                return this.getPieceDroppedMessage(game, effect);
            case EffectKind.PieceEnlisted:
                return this.getPieceEnlistedMessage(game, effect);
            case EffectKind.PieceKilled:
                return this.getPieceKilledMessage(game, effect);
            case EffectKind.PieceMoved:
                return this.getPieceMovedMessage(game, effect);
            case EffectKind.PieceVacated:
                return this.getPieceVacatedMessage(game, effect);
            case EffectKind.PlayerAdded:
                return this.getPlayerAddedMessage(game, effect);
            case EffectKind.PlayerOutOfMoves:
                return this.getPlayerOutOfMovesMessages(game, effect);
            case EffectKind.PlayerRemoved:
                return this.getPlayerRemovedMessage(game, effect);
            case EffectKind.PlayerStatusChanged:
                return this.getPlayerStatusChangedMessage(game, effect);
            case EffectKind.TurnCycleAdvanced:
                return this.getTurnCycleAdvancedMessage(game, effect);
            case EffectKind.TurnCyclePlayerFellFromPower:
                return this.getTurnCyclePlayerFellFromPowerMessage(game, effect);
            case EffectKind.TurnCyclePlayerRemoved:
                return this.getTurnCyclePlayerRemovedMessage(game, effect);
            case EffectKind.TurnCyclePlayerRoseToPower:
                return this.getTurnCyclePlayerRoseToPowerMessage(game, effect);
            default:
                throw "Unsupported effect kind.";
        }
    }

    private getGameStatusChangedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as GameStatusChangedEffect;
        return Sprintf.sprintf(template, {
            oldStatus: f.oldValue,
            newStatus: f.newValue
        });
    }

    private getNeutralPlayerAddedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as NeutralPlayerAddedEffect;
        return Sprintf.sprintf(template, {
            player: f.name
        });
    }

    private getPieceAbandonedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceAbandonedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece, game)
        });
    }

    private getPieceDroppedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceDroppedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece, game),
            newCell: this.getCellLabel(f.newCellId, game.parameters.regionCount)
        });
    }

    private getPieceEnlistedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceEnlistedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece, game),
            newPlayer: this.getPlayerLabel(f.newPlayerId, game)
        });
    }

    private getPieceKilledMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceKilledEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece, game)
        });
    }

    private getPieceMovedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceMovedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece, game),
            oldCell : this.getCellLabel(f.oldPiece.cellId, game.parameters.regionCount),
            newCell : this.getCellLabel(f.newCellId, game.parameters.regionCount)
        });
    }

    private getPieceVacatedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceVacatedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece, game),
            newCell: f.newCellId.toString(),
            center: this.theme.getCenterCellName()
        });
    }

    private getPlayerAddedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerAddedEffect;
        return Sprintf.sprintf(template, {
            player: f.name
        });
    }

    private getPlayerOutOfMovesMessages(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerOutOfMovesEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerLabel(f.playerId, game)
        });
    }

    private getPlayerRemovedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerRemovedEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerLabel(f.playerId, game)
        });
    }

    private getPlayerStatusChangedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerStatusChangedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId, game),
            oldStatus: f.oldStatus,
            newStatus: f.newStatus
        });
    }

    private getTurnCycleAdvancedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCycleAdvancedEffect;
        return Sprintf.sprintf(template, {
            newCycle: f.newValue.map(id => this.getPlayerLabel(id, game)).join(",")
        });
    }

    private getTurnCyclePlayerFellFromPowerMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerFellFromPowerEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId, game),
            newCycle: f.newValue.map(id => this.getPlayerLabel(id, game)).join(",")
        });
    }

    private getTurnCyclePlayerRemovedMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerRemovedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId, game),
            newCycle: f.newValue.map(id => this.getPlayerLabel(id, game)).join(",")
        });
    }

    private getTurnCyclePlayerRoseToPowerMessage(game : Game, effect : Effect) : string {
        const template = this.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerRoseToPowerEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId, game),
            newCycle: f.newValue.map(id => this.getPlayerLabel(id, game)).join(",")
        });
    }

    //------
}