import * as React from 'react';
import '../../index.css';
import { Game, Event } from '../../api/model';
import ThemeService from '../../themes/themeService';
import GameHistoryTable from '../gameHistoryTable';

export interface HistoryPanelProps {
    game : Game,
    theme : ThemeService,
    events : Event[]
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        return (
            <div className="thinBorder">
                History
                <GameHistoryTable
                    game={this.props.game}
                    theme={this.props.theme}
                    events={this.props.events}
                />
            </div>
        );
    }
}