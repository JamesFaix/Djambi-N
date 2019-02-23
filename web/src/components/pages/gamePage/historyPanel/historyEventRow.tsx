import * as React from 'react';
import HistoryEffectRow from './historyEffectRow';
import {
    Board,
    Effect,
    Event,
    Game,
    Player
    } from '../../../../api/model';
import { Kernel as K } from '../../../../kernel';

export interface HistoryEventRowProps {
    game : Game,
    event : Event,
    isEffectVisible : (f : Effect) => boolean,
    textStyle : React.CSSProperties
    getBoard : (regionCount : number) => Board
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
                                    {K.copy.getEventMessage(this.props.game, e)}
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
                                        getBoard={n => this.props.getBoard(n)}
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
}