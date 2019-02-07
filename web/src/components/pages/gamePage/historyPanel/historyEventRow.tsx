import * as React from 'react';
import { Event, EventKind, Effect, Game, EffectKind, PlayerRemovedEffect, Player } from '../../../../api/model';
import DateService from '../../../../dateService';
import HistoryEffectRow from './historyEffectRow';
import * as Sprintf from 'sprintf-js';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';

export interface HistoryEventRowProps {
    game : Game,
    event : Event,
    theme : ThemeService,
    isEffectVisible : (f : Effect) => boolean,
    textStyle : React.CSSProperties
}

export default class HistoryEventRow extends React.Component<HistoryEventRowProps> {

    render() {
        const e = this.props.event;
        const player = this.getActingPlayer();
        let style;
        if (player !== null) {
            const color = this.props.theme.getPlayerColor(player.colorId);
            style = Styles.playerGlow(color);
        }
        return (
            <tr style={style}>
                <td style={Styles.noPadding}>
                    <table style={{width: "100%"}}>
                        <tbody>
                            <tr>
                                <td className={Classes.borderless} style={{padding: "10px 10px 0px 10px", width: "50%"}}>
                                    {this.getEventMessage(this.props.game, e)}
                                </td>
                                <td className={Classes.combine([Classes.borderless, Classes.lightText])} style={{padding: "10px 10px 0px 10px", width: "50%", textAlign: "right"}}>
                                    {DateService.format(e.createdOn)}
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div className={Classes.indented} style={this.props.textStyle}>
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

    private getActingPlayer() : Player {
        if (this.props.event.actingPlayerId === null) {
            return null;
        }
        return this.props.game.players.find(p => p.id === this.props.event.actingPlayerId);
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