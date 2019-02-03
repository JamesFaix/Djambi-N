import * as React from 'react';
import { Effect, EffectKind, PlayerKind, GameStatusChangedEffect, PieceKilledEffect, PieceMovedEffect, PiecesOwnershipChangedEffect, PlayerAddedEffect, PlayerEliminatedEffect, PlayerOutOfMovesEffect, PlayersRemovedEffect, TurnCycleChangedEffect } from '../../api/model';
import * as Sprintf from 'sprintf-js';

export interface HistoryEffectRowProps {
    effect : Effect
}

export default class HistoryEffectRow extends React.Component<HistoryEffectRowProps> {

    render() {
        const f = this.props.effect;
        return (
            <p>
                {this.getEffectMessage(f)}
            </p>
        );
    }

    private getEffectMessage(effect : Effect) : string {
        let f;
        switch (effect.kind) {
            case EffectKind.GameStatusChanged:
                f = effect.value as GameStatusChangedEffect;
                return Sprintf.sprintf("Game status changed from %s to %s.", f.oldValue, f.newValue);

            case EffectKind.PieceKilled:
                f = effect.value as PieceKilledEffect;
                return Sprintf.sprintf("Piece %i was killed.", f.pieceId);

            case EffectKind.PieceMoved:
                f = effect.value as PieceMovedEffect;
                return Sprintf.sprintf("Piece %i moved from cell %i to cell %i.", f.pieceId, f.oldCellId, f.newCellId);

            case EffectKind.PiecesOwnershipChanged:
                f = effect.value as PiecesOwnershipChangedEffect;
                if (f.newPlayerId === null) {
                    return Sprintf.sprintf("Pieces %s were abandoned by player %s.", f.pieceIds.join(", "), f.oldPlayerId);
                } else {
                    return Sprintf.sprintf("Pieces %s were enlisted by player %s.", f.pieceIds.join(", "), f.newPlayerId);
                }

            case EffectKind.PlayerAdded:
                f = effect.value as PlayerAddedEffect;
                if (f.playerRequest.kind === PlayerKind.Neutral) {
                    return Sprintf.sprintf("Neutral player %s joined the game.", f.playerRequest.name);
                } else {
                    return Sprintf.sprintf("Player %s joined the game.", f.playerRequest.name);
                }

            case EffectKind.PlayerEliminated:
                f = effect.value as PlayerEliminatedEffect;
                return Sprintf.sprintf("Player %s was eliminated.", f.playerId);

            case EffectKind.PlayerOutOfMoves:
                f = effect.value as PlayerOutOfMovesEffect;
                return Sprintf.sprintf("Player %s is out of moves.", f.playerId);

            case EffectKind.PlayersRemoved:
                f = effect.value as PlayersRemovedEffect;
                return Sprintf.sprintf("Players %s were removed from the game.", f.playerIds.join(", "));

            case EffectKind.TurnCycleChanged:
                f = effect.value as TurnCycleChangedEffect;
                return Sprintf.sprintf("The turn cycle was changed from [%s] to [%s].", f.oldValue.join(","), f.newValue.join(","))

            default:
                throw "Unsupported effect kind.";
        }
    }
}