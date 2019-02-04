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
        let f;
        switch (effect.kind) {
            case Model.EffectKind.GameStatusChanged:
                return this.getGameStatusChangedMessage(game, effect.value as Model.GameStatusChangedEffect);
            case Model.EffectKind.PieceKilled:
                return this.getPieceKilledMessage(game, effect.value as Model.PieceKilledEffect);
            case Model.EffectKind.PieceMoved:
                return this.getPieceMovedMessage(game, effect.value as Model.PieceMovedEffect);
            case Model.EffectKind.PieceOwnershipChanged:
                return this.getPieceOwnershipChangedMessage(game, effect.value as Model.PieceOwnershipChangedEffect);
            case Model.EffectKind.PlayerAdded:
                return this.getPlayerAddedMessage(game, effect.value as Model.PlayerAddedEffect);
            case Model.EffectKind.PlayerEliminated:
                return this.getPlayerEliminatedMessage(game, effect.value as Model.PlayerEliminatedEffect);
            case Model.EffectKind.PlayerOutOfMoves:
                return this.getPlayerOutOfMovesMessages(game, effect.value as Model.PlayerOutOfMovesEffect);
            case Model.EffectKind.PlayerRemoved:
                return this.getPlayerRemovedMessage(game, effect.value as Model.PlayerRemovedEffect);
            case Model.EffectKind.TurnCycleChanged:
                return this.getTurnCycleChangedMessage(game, effect.value as Model.TurnCycleChangedEffect);
            default:
                throw "Unsupported effect kind.";
        }
    }

    //---Specific effects---

    private getGameStatusChangedMessage(game : Model.Game, effect : Model.GameStatusChangedEffect) : string {
        return Sprintf.sprintf("Game status changed from %s to %s.", effect.oldValue, effect.newValue);
    }

    private getPieceKilledMessage(game : Model.Game, effect : Model.PieceKilledEffect) : string {
        return Sprintf.sprintf("%s was killed.", this.getPieceIdentifier(effect.oldPiece));
    }

    private getPieceMovedMessage(game : Model.Game, effect : Model.PieceMovedEffect) : string {
        const piece = this.getPieceIdentifier(effect.oldPiece);

        return Sprintf.sprintf("%s moved from cell %i to cell %i.", piece, effect.oldPiece.cellId, effect.newCellId);
    }

    private getPieceOwnershipChangedMessage(game : Model.Game, effect : Model.PieceOwnershipChangedEffect) : string {
        const piece = this.getPieceIdentifier(effect.oldPiece);

        if (effect.newPlayerId === null) {
            return Sprintf.sprintf("%s was abandoned.", piece);
        } else {
            return Sprintf.sprintf("%s was enlisted by player %s.", piece, effect.newPlayerId);
        }
    }

    private getPlayerAddedMessage(game : Model.Game, effect : Model.PlayerAddedEffect) : string {
        if (effect.kind === Model.PlayerKind.Neutral) {
            return Sprintf.sprintf("Neutral player %s joined the game.", effect.name);
        } else {
            return Sprintf.sprintf("Player %s joined the game.", effect.name);
        }
    }

    private getPlayerEliminatedMessage(game : Model.Game, effect : Model.PlayerEliminatedEffect) : string {
        return Sprintf.sprintf("Player %s was eliminated.", effect.playerId);
    }

    private getPlayerOutOfMovesMessages(game : Model.Game, effect : Model.PlayerOutOfMovesEffect) : string {
        return Sprintf.sprintf("Player %s is out of moves.", effect.playerId);
    }

    private getPlayerRemovedMessage(game : Model.Game, effect : Model.PlayerRemovedEffect) : string {
        return Sprintf.sprintf("Player %i was removed from the game.", effect.playerId);
    }

    private getTurnCycleChangedMessage(game : Model.Game, effect : Model.TurnCycleChangedEffect) : string {
        return Sprintf.sprintf("The turn cycle was changed from [%s] to [%s].", effect.oldValue.join(","), effect.newValue.join(","));
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
}