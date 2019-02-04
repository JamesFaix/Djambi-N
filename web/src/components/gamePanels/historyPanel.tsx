import * as React from 'react';
import '../../index.css';
import { Game, Event } from '../../api/model';
import HistoryTable from './historyTable';
import ThemeService from '../../themes/themeService';

export interface HistoryPanelProps {
    game : Game,
    events : Event[],
    theme : ThemeService
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        return (
            <div className="thinBorder">
                History
                <HistoryTable
                    game={this.props.game}
                    events={this.props.events}
                    theme={this.props.theme}
                />
            </div>
        );
    }
}