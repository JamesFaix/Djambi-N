import * as React from 'react';
import { Event, Game } from "../../api/model";
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import GameHistoryEventBox from './gameHistoryEventBox';
import { PlayHeader } from '../controls/headers';

interface GameHistorySectionProps {
    game : Game,
    history : Event[]
}

class gameHistorySection extends React.Component<GameHistorySectionProps> {
    render() {
        if (!this.props.history || !this.props.game) {
            return null;
        }

        const style : React.CSSProperties = {
            display: "flex",
            flexDirection: "column",
        };

        return (
            <div style={style}>
                <PlayHeader text="History"/>
                {
                    this.props.history.map((e, i) => {
                        return (
                            <GameHistoryEventBox
                                event={e}
                                game={this.props.game}
                                key={i}
                            />
                        );
                    })
                }
            </div>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game,
        history: state.activeGame.history
    };
};

const GameHistorySection = connect(mapStateToProps)(gameHistorySection);

export default GameHistorySection;