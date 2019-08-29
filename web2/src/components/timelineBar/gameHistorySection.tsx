import * as React from 'react';
import { Event, Game } from "../../api/model";
import { State } from '../../store/root';
import { connect } from 'react-redux';
import GameHistoryEventBox from './gameHistoryEventBox';
import { PlayHeader } from '../controls/headers';
import { Styles } from '../../styles/styles';

interface GameHistorySectionProps {
    game : Game,
    history : Event[]
}

class gameHistorySection extends React.Component<GameHistorySectionProps> {
    render() {
        if (!this.props.history || !this.props.game) {
            return null;
        }

        return (
            <div style={Styles.historyContainer}>
                <PlayHeader text="Past"/>
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

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game,
        history: state.activeGame.history
    };
};

const GameHistorySection = connect(mapStateToProps)(gameHistorySection);

export default GameHistorySection;