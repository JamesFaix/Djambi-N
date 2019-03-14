import * as React from 'react';
import HistoryTable from '../tables/historyTable';
import Scrollbars from 'react-custom-scrollbars';
import { Board, Event, Game, User } from '../../api/model';
import { Kernel as K } from '../../kernel';
import CurrentTurnPanel from './currentTurnPanel';

export interface HistoryPanelProps {
    game : Game,
    events : Event[],
    height? : string,
    width : string,
    textStyle : React.CSSProperties,
    getBoard : (regionCount : number) => Board,
    user : User
}

export default class HistoryPanel extends React.Component<HistoryPanelProps> {
    render() {
        let panelStyle = K.styles.combine([
            K.styles.flex(1),
            K.styles.width(this.props.width)
        ]);
        if (this.props.height) {
            panelStyle = K.styles.combine([panelStyle, K.styles.width(this.props.height)]);
        }

        const scrollStyle = K.styles.height("100%");

        return (
            <div className={K.classes.thinBorder} style={panelStyle}>
                History
                <Scrollbars style={scrollStyle}>
                    <CurrentTurnPanel
                        game={this.props.game}
                        width={this.props.width}
                        height={null}
                        user={this.props.user}
                        textStyle={this.props.textStyle}
                    />
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