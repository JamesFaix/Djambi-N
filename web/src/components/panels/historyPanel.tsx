import * as React from 'react';
import HistoryTable from '../tables/historyTable';
import Scrollbars from 'react-custom-scrollbars';
import { Board, Event, Game } from '../../api/model';
import { Kernel as K } from '../../kernel';

export interface HistoryPanelProps {
    game : Game,
    events : Event[],
    height : string,
    width : string,
    textStyle : React.CSSProperties,
    getBoard : (regionCount : number) => Board
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        const panelStyle = K.styles.combine([
            K.styles.height(this.props.height),
            K.styles.width(this.props.width)
        ]);
        return (
            <div className={K.classes.thinBorder} style={panelStyle}>
                History
                <Scrollbars style={K.styles.height(this.props.height)}>
                    <HistoryTable
                        game={this.props.game}
                        events={this.props.events}
                        textStyle={this.props.textStyle}
                        getBoard={n => this.props.getBoard(n)}
                    />
                </Scrollbars>
            </div>
        );
    }
}