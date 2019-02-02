import * as React from 'react';
import { Game, Event, Effect, EventKind, EffectKind, DiffEffect, GameStatus, ScalarEffect, DiffWithContextEffect, CreatePlayerRequest, PlayerKind } from '../api/model';
import ThemeService from '../themes/themeService';
import * as Sprintf from 'sprintf-js';
import HintCell from './tables/hintCell';
import EmphasizedTextCell from './tables/emphasizedTextCell';
import moment = require("moment");

export interface GameHistoryTableProps {
    game : Game,
    events : Event[],
    theme : ThemeService
}

export default class GameHistoryTable extends React.Component<GameHistoryTableProps> {

    render() {
        return (
            <div style={{display:"flex"}}>
                <table className="table">
                    <tbody>
                        {
                            this.props.events
                                .filter(e => this.isVisibleEvent(e))
                                .map((e, i) => this.renderRow(e, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(event : Event, rowNumber : number) {
        return (
            <tr key={"row" + rowNumber}>
                <td>
                    <table>
                        <tbody>
                            <tr>
                                <EmphasizedTextCell text={this.getEventMessage(event)}/>
                                <HintCell text={moment(event.createdOn).format('MM/DD/YY hh:mm a')}/>
                            </tr>
                        </tbody>
                    </table>
                    <div className="indented">
                        {
                            event.effects
                                .filter(f => this.isVisibleEffect(f))
                                .map((f, i) =>
                                    <p key={"effect" + i}>
                                        {this.getEffectMessage(f)}
                                    </p>
                                )
                        }
                    </div>
                </td>
            </tr>
        );
    }

    private isVisibleEvent(event : Event) : boolean {
        switch (event.kind) {
            case EventKind.CellSelected:
            case EventKind.GameCanceled:
            case EventKind.GameParametersChanged:
            case EventKind.PlayerJoined:
            case EventKind.TurnReset:
                return false;

            default:
                return true;
        }
    }

    private isVisibleEffect(effect : Effect) : boolean {
        switch (effect.kind) {
            case EffectKind.CurrentTurnChanged:
            case EffectKind.ParametersChanged:
                return false;

            default:
                return true;
        }
    }

    private getEventMessage(event : Event) : string {
        switch (event.kind) {
            case EventKind.GameStarted:
                return "Game started";

            case EventKind.PlayerEjected:
                return "Player ejected";

            case EventKind.PlayerQuit:
                return "Player quit";

            case EventKind.TurnCommitted:
                return "Turn committed";

            default:
                throw "Unsupported event kind.";
        }
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