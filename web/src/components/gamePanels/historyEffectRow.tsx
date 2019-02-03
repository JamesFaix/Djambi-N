import * as React from 'react';
import { Effect, DiffEffect, GameStatus, ScalarEffect, DiffWithContextEffect, CreatePlayerRequest, EffectKind, PlayerKind } from '../../api/model';
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
                f = effect.value as DiffEffect<GameStatus>;
                return Sprintf.sprintf("Game status changed from %s to %s.", f.oldValue, f.newValue);

            case EffectKind.PieceKilled:
                f = effect.value as ScalarEffect<number>;
                return Sprintf.sprintf("Piece %i was killed.", f.value);

            case EffectKind.PieceMoved:
                f = effect.value as DiffWithContextEffect<number, number>;
                return Sprintf.sprintf("Piece %i moved from cell %i to cell %i.", f.context, f.oldValue, f.newValue);

            case EffectKind.PiecesOwnershipChanged:
                f = effect.value as DiffWithContextEffect<number, number[]>;
                if (f.newValue === null) {
                    return Sprintf.sprintf("Pieces %s were abandoned by player %s.", f.context.join(", "), f.oldValue);
                } else {
                    return Sprintf.sprintf("Pieces %s were enlisted by player %s.", f.context.join(", "), f.newValue);
                }

            case EffectKind.PlayerAdded:
                f = effect.value as ScalarEffect<CreatePlayerRequest>;
                if (f.value.kind === PlayerKind.Neutral) {
                    return Sprintf.sprintf("Neutral player %s joined the game.", f.value.name);
                } else {
                    return Sprintf.sprintf("Player %s joined the game.", f.value.name);
                }

            case EffectKind.PlayerEliminated:
                f = effect.value as ScalarEffect<number>;
                return Sprintf.sprintf("Player %s was eliminated.", f.value);

            case EffectKind.PlayerOutOfMoves:
                f = effect.value as ScalarEffect<number>;
                return Sprintf.sprintf("Player %s is out of moves.", f.value);

            case EffectKind.PlayersRemoved:
                f = effect.value as ScalarEffect<number[]>;
                return Sprintf.sprintf("Players %s were removed from the game.", f.value.join(", "));

            case EffectKind.TurnCycleChanged:
                f = effect.value as DiffEffect<number[]>;
                return Sprintf.sprintf("The turn cycle was changed from [%s] to [%s].", f.oldValue.join(","), f.newValue.join(","))

            default:
                throw "Unsupported effect kind.";
        }
    }
}