import * as React from 'react';
import * as Sprintf from 'sprintf-js';
import ThemeService from '../../../../themes/themeService';
import { Game, Effect, EffectKind, GameStatusChangedEffect, PieceKilledEffect, PlayerAddedEffect, PieceMovedEffect, PlayerKind, PlayerEliminatedEffect, PlayerOutOfMovesEffect, PlayerRemovedEffect, PieceKind, Piece, NeutralPlayerAddedEffect, PieceAbandonedEffect, PieceEnlistedEffect, PieceDroppedEffect, PieceVacatedEffect, TurnCycleAdvancedEffect, TurnCyclePlayerFellFromPowerEffect, TurnCyclePlayerRemovedEffect, TurnCyclePlayerRoseToPowerEffect } from '../../../../api/model';

export interface HistoryEffectRowProps {
    game : Game,
    effect : Effect,
    theme : ThemeService
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
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as GameStatusChangedEffect;
        return Sprintf.sprintf(template, {
            oldStatus: f.oldValue,
            newStatus: f.newValue
        });
    }

    private getNeutralPlayerAddedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as NeutralPlayerAddedEffect;
        return Sprintf.sprintf(template, {
            player: f.name
        })
    }

    private getPieceAbandonedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceAbandonedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece)
        });
    }

    private getPieceDroppedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceDroppedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece),
            newCell: f.newCellId.toString()
        });
    }

    private getPieceEnlistedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceEnlistedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece),
            newPlayer: this.getPlayerName(f.newPlayerId)
        });
    }

    private getPieceKilledMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceKilledEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece)
        });
    }

    private getPieceMovedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceMovedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece),
            oldCell : f.oldPiece.cellId.toString(),
            newCell : f.newCellId.toString()
        });
    }

    private getPieceVacatedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PieceVacatedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece),
            newCell: f.newCellId.toString(),
            center: this.props.theme.getCenterCellName()
        });
    }

    private getPlayerAddedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerAddedEffect;
        return Sprintf.sprintf(template, {
            player: f.name
        });
    }

    private getPlayerEliminatedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerEliminatedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerName(f.playerId)
        });
    }

    private getPlayerOutOfMovesMessages(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerOutOfMovesEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerName(f.playerId)
        });
    }

    private getPlayerRemovedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as PlayerRemovedEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerName(f.playerId)
        });
    }

    private getTurnCycleAdvancedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCycleAdvancedEffect;
        return Sprintf.sprintf(template, {
            newCycle: f.newValue.map(id => this.getPlayerName(id)).join(",")
        });
    }

    private getTurnCyclePlayerFellFromPowerMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerFellFromPowerEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerName(f.playerId),
            newCycle: f.newValue.map(id => this.getPlayerName(id)).join(",")
        });
    }

    private getTurnCyclePlayerRemovedMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerRemovedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerName(f.playerId),
            newCycle: f.newValue.map(id => this.getPlayerName(id)).join(",")
        });
    }

    private getTurnCyclePlayerRoseToPowerMessage(game : Game, effect : Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as TurnCyclePlayerRoseToPowerEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerName(f.playerId),
            newCycle: f.newValue.map(id => this.getPlayerName(id)).join(",")
        });
    }

    //---Helpers---

    private getPieceIdentifier(piece : Piece) : string {
        const kindName = this.props.theme.getPieceName(piece.kind);
        if (piece.kind === PieceKind.Corpse) {
            return kindName;
        }

        const player = this.props.game.players.find(p => p.id === piece.playerId);
        return player
            ? Sprintf.sprintf("%s's %s", player.name, kindName)
            : Sprintf.sprintf("Neutral %s", kindName);
    }

    private getPlayerName(playerId : number) : string {
        return this.props.game.players.find(p => p.id === playerId).name;
    }
}