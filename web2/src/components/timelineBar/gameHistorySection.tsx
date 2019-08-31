import * as React from 'react';
import { Event, Game, Board } from "../../api/model";
import { State } from '../../store/root';
import { connect } from 'react-redux';
import GameHistoryEventBox from './gameHistoryEventBox';
import { TimelineHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import { Icons } from '../../utilities/icons';

interface GameHistorySectionProps {
    game : Game,
    history : Event[],
    board : Board
}

const gameHistorySection : React.SFC<GameHistorySectionProps> = props => {
    if (!props.history || !props.game) {
        return null;
    }

    return (
        <div
            id="history-section"
            className={Classes.timelineBarHistory}
        >
            <TimelineHeader icon={Icons.Timeline.history}/>
            {
                props.history.map((e, i) => {
                    return (
                        <GameHistoryEventBox
                            event={e}
                            game={props.game}
                            key={i}
                            board={props.board}
                        />
                    );
                })
            }
        </div>
    );
}

const mapStateToProps = (state : State) => {
    if (state.activeGame.game){
        return {
            game: state.activeGame.game,
            history: state.activeGame.history,
            board : state.boards.boards.get(state.activeGame.game.parameters.regionCount)
        };
    } else {
        return {
            game : null,
            history : null,
            board : null
        };
    }
};

const GameHistorySection = connect(mapStateToProps)(gameHistorySection);

export default GameHistorySection;