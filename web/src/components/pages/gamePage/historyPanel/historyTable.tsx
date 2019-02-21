import * as React from 'react';
import { Game, Event, Effect, EventKind, EffectKind, Board } from '../../../../api/model';
import HistoryEventRow from './historyEventRow';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';

export interface HistoryTableProps {
    game : Game,
    events : Event[],
    theme : ThemeService,
    textStyle : React.CSSProperties,
    getBoard : (regionCount : number) => Board
}

export default class HistoryTable extends React.Component<HistoryTableProps> {

    render() {
        return (
            <div className={Classes.flex}>
                <table
                    className={Classes.table}
                    style={Styles.width("100%")}
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
                                        theme={this.props.theme}
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

    private isEffectVisible(effect : Effect) : boolean {
        switch (effect.kind) {
            case EffectKind.CurrentTurnChanged:
            case EffectKind.ParametersChanged:
                return false;

            default:
                return true;
        }
    }
}