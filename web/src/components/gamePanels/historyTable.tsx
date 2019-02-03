import * as React from 'react';
import { Game, Event, Effect, EventKind, EffectKind } from '../../api/model';
import HistoryEventRow from './historyEventRow';

export interface HistoryTableProps {
    game : Game,
    events : Event[]
}

export default class HistoryTable extends React.Component<HistoryTableProps> {

    render() {
        return (
            <div style={{display:"flex"}}>
                <table className="table" style={{width:"100%"}}>
                    <tbody>
                        {
                            this.props.events
                                .filter(e => this.isEventVisible(e))
                                .map((e, i) =>
                                    <HistoryEventRow
                                        key={"event" + i}
                                        event={e}
                                        isEffectVisible={f => this.isEffectVisible(f)}
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