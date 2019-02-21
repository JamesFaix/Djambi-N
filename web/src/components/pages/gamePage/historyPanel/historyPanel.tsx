import * as React from 'react';
import { Game, Event, Board } from '../../../../api/model';
import HistoryTable from './historyTable';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';
import Scrollbars from 'react-custom-scrollbars';

export interface HistoryPanelProps {
    game : Game,
    events : Event[],
    theme : ThemeService,
    height : string,
    width : string,
    textStyle : React.CSSProperties,
    getBoard : (regionCount : number) => Board
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        const panelStyle = Styles.combine([
            Styles.height(this.props.height),
            Styles.width(this.props.width)
        ]);
        return (
            <div className={Classes.thinBorder} style={panelStyle}>
                History
                <Scrollbars style={Styles.height(this.props.height)}>
                    <HistoryTable
                        game={this.props.game}
                        events={this.props.events}
                        theme={this.props.theme}
                        textStyle={this.props.textStyle}
                        getBoard={n => this.props.getBoard(n)}
                    />
                </Scrollbars>
            </div>
        );
    }
}