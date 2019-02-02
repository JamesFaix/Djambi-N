import * as React from 'react';
import { Game, Event, EventKind, Effect } from '../../api/model';
import ThemeService from '../../themes/themeService';
import HintCell from '../tables/hintCell';
import EmphasizedTextCell from '../tables/emphasizedTextCell';
import DateService from '../../dateService';
import HistoryEffectRow from './historyEffectRow';

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
                <td>
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
                                        event={this.props.event}
                                        game={this.props.game}
                                        theme={this.props.theme}
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