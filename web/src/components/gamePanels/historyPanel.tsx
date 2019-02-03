import * as React from 'react';
import '../../index.css';
import { Game, Event } from '../../api/model';
import HistoryTable from './historyTable';

export interface HistoryPanelProps {
    game : Game,
    events : Event[]
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        return (
            <div className="thinBorder">
                History
                <HistoryTable
                    game={this.props.game}
                    events={this.props.events}
                />
            </div>
        );
    }
}