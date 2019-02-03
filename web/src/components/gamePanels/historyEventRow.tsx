import * as React from 'react';
import { Event, EventKind, Effect } from '../../api/model';
import HintCell from '../tables/hintCell';
import EmphasizedTextCell from '../tables/emphasizedTextCell';
import DateService from '../../dateService';
import HistoryEffectRow from './historyEffectRow';

export interface HistoryEventRowProps {
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
                                <EmphasizedTextCell text={this.getEventMessage(e)}/>
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
}