import * as React from 'react';
import { Event, EventKind, Effect, Game, EffectKind, PlayerRemovedEffect } from '../../api/model';
import HintCell from '../tables/hintCell';
import EmphasizedTextCell from '../tables/emphasizedTextCell';
import DateService from '../../dateService';
import HistoryEffectRow from './historyEffectRow';
import * as Sprintf from 'sprintf-js';

export interface HistoryEventRowProps {
    game : Game,
    event : Event,
    isEffectVisible : (f : Effect) => boolean
}

export default class HistoryEventRow extends React.Component<HistoryEventRowProps> {

    render() {
        const e = this.props.event;
        return (
            <tr>
                <td style={{padding:0}}>
                    <table>
                        <tbody>
                            <tr>
                                <EmphasizedTextCell text={this.getEventMessage(this.props.game, e)}/>
                                <HintCell text={DateService.format(e.createdOn)}/>
                            </tr>
                        </tbody>
                    </table>
                    <div className="indented">
                        {
                            e.effects
                                .filter(f => this.props.isEffectVisible(f))
                                .map((f, i) =>
                                    <HistoryEffectRow
                                        key={"effect" + i}
                                        effect={f}
                                    />
                                )
                        }
                    </div>
                </td>
            </tr>
        );
    }

    private getEventMessage(game : Game, event : Event) : string {
        const actingPlayer = game.players.find(p => p.id === event.actingPlayerId);
        console.log(actingPlayer);
        const actingPlayerName = actingPlayer ? actingPlayer.name : "[Admin]";
        let params;
        switch (event.kind) {
            case EventKind.GameStarted:
                return "Game started";

            case EventKind.PlayerEjected:
                const effect = event.effects.find(f => f.kind === EffectKind.PlayerRemoved).value as PlayerRemovedEffect;
                const removedPlayer = game.players.find(p => p.id === effect.playerId);
                params = {
                    actingPlayer: actingPlayerName,
                    removedPlayer: removedPlayer.name
                };
                return Sprintf.sprintf("%(actingPlayer)s ejected %(removedPlayer)s", params);

            case EventKind.PlayerQuit:
                params = {
                    player: actingPlayerName
                };
                return Sprintf.sprintf("%(player)s conceded", params);

            case EventKind.TurnCommitted:
                params = {
                    player: actingPlayerName
                };
                return Sprintf.sprintf("%(player)s took a turn", params);

            default:
                throw "Unsupported event kind.";
        }
    }
}