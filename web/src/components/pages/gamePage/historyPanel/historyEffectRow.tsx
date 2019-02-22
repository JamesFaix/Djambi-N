import * as React from 'react';
import * as Sprintf from 'sprintf-js';
import { Game, Effect, EffectKind, GameStatusChangedEffect, PieceKilledEffect, PlayerAddedEffect, PieceMovedEffect, PlayerKind, PlayerEliminatedEffect, PlayerOutOfMovesEffect, PlayerRemovedEffect, PieceKind, Piece, NeutralPlayerAddedEffect, PieceAbandonedEffect, PieceEnlistedEffect, PieceDroppedEffect, PieceVacatedEffect, TurnCycleAdvancedEffect, TurnCyclePlayerFellFromPowerEffect, TurnCyclePlayerRemovedEffect, TurnCyclePlayerRoseToPowerEffect, Board } from '../../../../api/model';
import CopyService from '../../../../copyService';
import {Kernel as K} from '../../../../kernel';

export interface HistoryEffectRowProps {
    game : Game,
    effect : Effect,
    getBoard : (regionCount : number) => Board
}

export default class HistoryEffectRow extends React.Component<HistoryEffectRowProps> {

    render() {
        return (
            <p>
                {this.getEffectMessage(this.props.game, this.props.effect)}
            </p>
        );
    }

    private getEffectMessage(game : Game, effect : Effect) : string {
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
            case EffectKind.PlayerEliminated:
                return this.getPlayerEliminatedMessage(game, effect);
            case EffectKind.PlayerOutOfMoves:
                return this.getPlayerOutOfMovesMessages(game, effect);
            case EffectKind.PlayerRemoved:
                return this.getPlayerRemovedMessage(game, effect);
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

    //---Specific effects---

    private getGameStatusChangedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as GameStatusChangedEffect;
        return Sprintf.sprintf(template, {
            oldStatus: f.oldValue,
            newStatus: f.newValue
        });
    }

    private getNeutralPlayerAddedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as NeutralPlayerAddedEffect;
        return Sprintf.sprintf(template, {
            player: f.name
        })
    }

    private getPieceAbandonedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceAbandonedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece)
        });
    }

    private getPieceDroppedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceDroppedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece),
            newCell: this.getCellLabel(f.newCellId)
        });
    }

    private getPieceEnlistedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceEnlistedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece),
            newPlayer: this.getPlayerLabel(f.newPlayerId)
        });
    }

    private getPieceKilledMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceKilledEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece)
        });
    }

    private getPieceMovedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceMovedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece),
            oldCell : this.getCellLabel(f.oldPiece.cellId),
            newCell : this.getCellLabel(f.newCellId)
        });
    }

    private getPieceVacatedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceVacatedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceLabel(f.oldPiece),
            newCell: f.newCellId.toString(),
            center: K.theme.getCenterCellName()
        });
    }

    private getPlayerAddedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerAddedEffect;
        return Sprintf.sprintf(template, {
            player: f.name
        });
    }

    private getPlayerEliminatedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerEliminatedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId)
        });
    }

    private getPlayerOutOfMovesMessages(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerOutOfMovesEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerLabel(f.playerId)
        });
    }

    private getPlayerRemovedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerRemovedEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerLabel(f.playerId)
        });
    }

    private getTurnCycleAdvancedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCycleAdvancedEffect;
        return Sprintf.sprintf(template, {
            newCycle: f.newValue.map(id => this.getPlayerLabel(id)).join(",")
        });
    }

    private getTurnCyclePlayerFellFromPowerMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerFellFromPowerEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId),
            newCycle: f.newValue.map(id => this.getPlayerLabel(id)).join(",")
        });
    }

    private getTurnCyclePlayerRemovedMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerRemovedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId),
            newCycle: f.newValue.map(id => this.getPlayerLabel(id)).join(",")
        });
    }

    private getTurnCyclePlayerRoseToPowerMessage(game : Game, effect : Effect) : string {
        const template = K.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerRoseToPowerEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerLabel(f.playerId),
            newCycle: f.newValue.map(id => this.getPlayerLabel(id)).join(",")
        });
    }

    //---Helpers---

    private getPieceLabel(piece : Piece) : string {
        const kindName = K.theme.getPieceName(piece.kind);
        if (piece.kind === PieceKind.Corpse) {
            return kindName;
        }

        const player = this.props.game.players.find(p => p.id === piece.playerId);
        return player
            ? Sprintf.sprintf("%s's %s", player.name, kindName)
            : Sprintf.sprintf("Neutral %s", kindName);
    }

    private getPlayerLabel(playerId : number) : string {
        return this.props.game.players.find(p => p.id === playerId).name;
    }

    private getCellLabel(cellId : number) : string {
        //The first time the board is needed it must be fetched from the API.
        //To avoid making this asynchronous, this just skips the API call if the board is not already cached.
        const board = this.props.getBoard(this.props.game.parameters.regionCount);
        if (board) {
            const cell = board.cells.find(c => c.id === cellId);
            return CopyService.locationToString(cell.locations[0]);
        } else {
            return cellId.toString();
        }
    }
}