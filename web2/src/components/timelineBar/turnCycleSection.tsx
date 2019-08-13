import * as React from 'react';
import { Game, Player } from '../../api/model';
import TurnCycleTurnBox from './turnCycleTurnBox';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import { PlayHeader } from '../controls/headers';

interface TurnCycleSectionProps {
    game: Game
}

class turnCycleSection extends React.Component<TurnCycleSectionProps> {
    render() {
        if (!this.props.game){
            return null;
        }

        const players = this.getTurnCyclePlayers();
        const style = {
            display: "flex"
        };
        return (
            <div>
                <PlayHeader text="Turn cycle"/>
                <div style={style}>
                    {players.map((p, i) => {
                        return <TurnCycleTurnBox player={p} key={i}/>;
                    })}
                </div>
            </div>
        );
    }

    private getTurnCyclePlayers() : Player[] {
        const players = this.props.game.players;
        const turns = this.props.game.turnCycle;
        return turns.map(pId => players.find(p => p.id === pId));
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game
    };
};

const TurnCycleSection = connect(mapStateToProps)(turnCycleSection);

export default TurnCycleSection;