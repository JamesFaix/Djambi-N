import * as React from 'react';
import * as Model from '../../api/model';
import * as Sprintf from 'sprintf-js';
import ThemeService from '../../themes/themeService';

export interface HistoryEffectRowProps {
    game : Model.Game,
    effect : Model.Effect,
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

    private getEffectMessage(game : Model.Game, effect : Model.Effect) : string {
        switch (effect.kind) {
            case Model.EffectKind.GameStatusChanged:
                return this.getGameStatusChangedMessage(game, effect);
            case Model.EffectKind.PieceKilled:
                return this.getPieceKilledMessage(game, effect);
            case Model.EffectKind.PieceMoved:
                return this.getPieceMovedMessage(game, effect);
            case Model.EffectKind.PieceOwnershipChanged:
                return this.getPieceOwnershipChangedMessage(game, effect);
            case Model.EffectKind.PlayerAdded:
                return this.getPlayerAddedMessage(game, effect);
            case Model.EffectKind.PlayerEliminated:
                return this.getPlayerEliminatedMessage(game, effect);
            case Model.EffectKind.PlayerOutOfMoves:
                return this.getPlayerOutOfMovesMessages(game, effect);
            case Model.EffectKind.PlayerRemoved:
                return this.getPlayerRemovedMessage(game, effect);
            case Model.EffectKind.TurnCycleChanged:
                return this.getTurnCycleChangedMessage(game, effect);
            default:
                throw "Unsupported effect kind.";
        }
    }

    //---Specific effects---

    private getGameStatusChangedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.GameStatusChangedEffect;
        return Sprintf.sprintf(template, {
            oldStatus: f.oldValue,
            newStatus: f.newValue
        });
    }

    private getPieceKilledMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PieceKilledEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece)
        });
    }

    private getPieceMovedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PieceMovedEffect;
        return Sprintf.sprintf(template, {
            piece: this.getPieceIdentifier(f.oldPiece),
            oldCell : f.oldPiece.cellId.toString(),
            newCell : f.newCellId.toString()
        });
    }

    private getPieceOwnershipChangedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PieceOwnershipChangedEffect;
        const piece = this.getPieceIdentifier(f.oldPiece);
        if (f.newPlayerId === null) {
            return Sprintf.sprintf(template, {
                piece: piece
            });
        } else {
            return Sprintf.sprintf(template, {
                piece: piece,
                newPlayer: this.getPlayerName(f.newPlayerId)
            });
        }
    }

    private getPlayerAddedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PlayerAddedEffect;
        if (f.kind === Model.PlayerKind.Neutral) {
            return Sprintf.sprintf(template, {
                player: f.name
            });
        } else {
            return Sprintf.sprintf(template, {
                player: f.name
            });
        }
    }

    private getPlayerEliminatedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PlayerEliminatedEffect;
        return Sprintf.sprintf(template, {
            player: this.getPlayerName(f.playerId)
        });
    }

    private getPlayerOutOfMovesMessages(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PlayerOutOfMovesEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerName(f.playerId)
        });
    }

    private getPlayerRemovedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.PlayerRemovedEffect;
        return Sprintf.sprintf(template,  {
            player: this.getPlayerName(f.playerId)
        });
    }

    private getTurnCycleChangedMessage(game : Model.Game, effect : Model.Effect) : string {
        const template = this.props.theme.getEffectMessageTemplate(effect);
        const f = effect.value as Model.TurnCycleChangedEffect;
        return Sprintf.sprintf(template, {
            oldCycle: f.oldValue.map(id => this.getPlayerName(id)).join(","),
            newCycle: f.newValue.map(id => this.getPlayerName(id)).join(",")
        });
    }

    //---Helpers---

    private getPieceIdentifier(piece : Model.Piece) : string {
        const kindName = this.props.theme.getPieceName(piece.kind);
        if (piece.kind === Model.PieceKind.Corpse) {
            return kindName;
        }

        const player = this.props.game.players.find(p => p.id === piece.playerId);
        const playerName = player ? player.name : "Neutral";
        return Sprintf.sprintf("%s's %s", playerName, kindName);
    }

    private getPlayerName(playerId : number) : string {
        return this.props.game.players.find(p => p.id === playerId).name;
    }
}