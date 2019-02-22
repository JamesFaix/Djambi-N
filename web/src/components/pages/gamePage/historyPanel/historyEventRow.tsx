import * as React from 'react';
import { Event, EventKind, Effect, Game, EffectKind, PlayerRemovedEffect, Player } from '../../../../api/model';
import HistoryEffectRow from './historyEffectRow';
import * as Sprintf from 'sprintf-js';
import {Kernel as K} from '../../../../kernel';

export interface HistoryEventRowProps {
    game : Game,
    event : Event,
    isEffectVisible : (f : Effect) => boolean,
    textStyle : React.CSSProperties
}

export default class HistoryEventRow extends React.Component<HistoryEventRowProps> {

    render() {
        const e = this.props.event;
        const player = this.getActingPlayer();
        let style;
        if (player !== null) {
            const color = K.theme.getPlayerColor(player.colorId);
            style = K.styles.playerGlow(color);
        }
        const cellStyle = K.styles.combine([K.styles.width("50%"), K.styles.padding("10px 10px 0px 10px")]);
        return (
            <tr style={style}>
                <td style={K.styles.noPadding}>
                    <table style={K.styles.width("100%")}>
                        <tbody>
                            <tr>
                                <td className={K.classes.borderless} style={cellStyle}>
                                    {this.getEventMessage(this.props.game, e)}
                                </td>
                                <td className={K.classes.combine([K.classes.borderless, K.classes.lightText, K.classes.rightAligned])} style={cellStyle}>
                                    {K.dates.format(e.createdOn)}
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div className={K.classes.indented} style={this.props.textStyle}>
                        {
                            e.effects
                                .filter(f => this.props.isEffectVisible(f))
                                .map((f, i) =>
                                    <HistoryEffectRow
                                        key={"effect" + i}
                                        game={this.props.game}
                                        effect={f}
                                    />
                                )
                        }
                    </div>
                </td>
            </tr>
        );
    }

    private getActingPlayer() : Player {
        if (this.props.event.actingPlayerId === null) {
            return null;
        }
        return this.props.game.players.find(p => p.id === this.props.event.actingPlayerId);
    }

    private getEventMessage(game : Game, event : Event) : string {
        const actingPlayer = game.players.find(p => p.id === event.actingPlayerId);
        const actingPlayerName = actingPlayer ? actingPlayer.name : "[Admin]";

        const template = K.theme.getEventMessageTemplate(event);

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