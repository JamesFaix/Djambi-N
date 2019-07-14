import * as React from 'react';
import HistoryEventRow from './historyEventRow';
import {
    Board,
    Effect,
    EffectKind,
    Event,
    EventKind,
    Game
} from '../../api/model';
import { Kernel as K } from '../../kernel';

export interface HistoryTableProps {
    game : Game,
    events : Event[],
    textStyle : React.CSSProperties,
    getBoard : (regionCount : number) => Board
}

export default class HistoryTable extends React.Component<HistoryTableProps> {

    render() {
        return (
            <div className={K.classes.flex}>
                <table
                    className={K.classes.table}
                    style={K.styles.width("100%")}
                >
                    <tbody>
                        {
                            this.props.events
                                .filter(e => this.isEventVisible(e))
                                .map((e, i) =>
                                    <HistoryEventRow
                                        key={"event" + i}
                                        game={this.props.game}
                                        event={e}
                                        isEffectVisible={f => this.isEffectVisible(f)}
                                        textStyle={this.props.textStyle}
                                        getBoard={n => this.props.getBoard(n)}
                                    />
                                )
                        }
                    </tbody>
                </table>
            </div>
        );
    }


    private isEventVisible(event : Event) : boolean {
        switch (event.kind) {
            case EventKind.GameStarted:
            case EventKind.TurnCommitted:
            case EventKind.PlayerStatusChanged:
                return true;
            default:
                return false;
        }
    }

    private isEffectVisible(effect : Effect) : boolean {
        switch (effect.kind) {
            case EffectKind.CurrentTurnChanged:
            case EffectKind.ParametersChanged:
            case EffectKind.PlayerRemoved:
                return false;

            default:
                return true;
        }
    }
}