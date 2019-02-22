import * as React from 'react';
import { Game, Event } from '../../../../api/model';
import HistoryTable from './historyTable';
import Scrollbars from 'react-custom-scrollbars';
import { Kernel as K } from '../../../../kernel';

export interface HistoryPanelProps {
    game : Game,
    events : Event[],
    height : string,
    width : string,
    textStyle : React.CSSProperties
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
                    />
                </Scrollbars>
            </div>
        );
    }
}