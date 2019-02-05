import * as React from 'react';
import '../../index.css';
import { Game, Event } from '../../api/model';
import HistoryTable from './historyTable';
import ThemeService from '../../themes/themeService';
import { Classes, Styles } from '../../styles';
import Scrollbars from 'react-custom-scrollbars';

export interface HistoryPanelProps {
    game : Game,
    events : Event[],
    theme : ThemeService,
    height : number
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        return (
            <div className={Classes.thinBorder}>
                History
                <Scrollbars style={Styles.absoluteHeight(this.props.height)}>
                    <HistoryTable
                        game={this.props.game}
                        events={this.props.events}
                        theme={this.props.theme}
                    />
                </Scrollbars>
            </div>
        );
    }
}