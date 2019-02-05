import * as React from 'react';
import { Event, EventKind, Effect, Game, EffectKind, PlayerRemovedEffect } from '../../api/model';
import HintCell from '../tables/hintCell';
import EmphasizedTextCell from '../tables/emphasizedTextCell';
import DateService from '../../dateService';
import HistoryEffectRow from './historyEffectRow';
import * as Sprintf from 'sprintf-js';
import ThemeService from '../../themes/themeService';
import { Classes, Styles } from '../../styles';

export interface HistoryEventRowProps {
    game : Game,
    event : Event,
    theme : ThemeService,
    isEffectVisible : (f : Effect) => boolean
}

export default class HistoryEventRow extends React.Component<HistoryEventRowProps> {

    render() {
        const e = this.props.event;
        return (
            <tr>
                <td style={Styles.noPadding}>
                    <table>
                        <tbody>
                            <tr>
                                <EmphasizedTextCell text={this.getEventMessage(this.props.game, e)}/>
                                <HintCell text={DateService.format(e.createdOn)}/>
                            </tr>
                        </tbody>
                    </table>
                    <div className={Classes.indented}>
                        {
                            e.effects
                                .filter(f => this.props.isEffectVisible(f))
                                .map((f, i) =>
                                    <HistoryEffectRow
                                        key={"effect" + i}
                                        game={this.props.game}
                                        effect={f}
                                        theme={this.props.theme}
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

        const template = this.props.theme.getEventMessageTemplate(event);

        switch (event.kind) {
            case EventKind.GameStarted:
                return template;

            case EventKind.PlayerEjected:
                const effect = event.effects.find(f => f.kind === EffectKind.PlayerRemoved).value as PlayerRemovedEffect;
                const removedPlayer = game.players.find(p => p.id === effect.playerId);
                return Sprintf.sprintf(template, {
                    actingPlayer: actingPlayerName,
                    removedPlayer: removedPlayer.name
                });

            case EventKind.PlayerQuit:
                return Sprintf.sprintf(template, {
                    player: actingPlayerName
                });

            case EventKind.TurnCommitted:
                return Sprintf.sprintf(template, {
                    player: actingPlayerName
                });

            default:
                throw "Unsupported event kind.";
        }
    }
}