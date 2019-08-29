import * as React from 'react';
import { Game, Player } from '../../api/model';
import TurnCycleTurnBox from './turnCycleTurnBox';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { PlayHeader } from '../controls/headers';
import { Styles } from '../../styles/styles';

interface TurnCycleSectionProps {
    game: Game
}

class turnCycleSection extends React.Component<TurnCycleSectionProps> {
    render() {
        if (!this.props.game){
            return null;
        }

        const players = this.getTurnCyclePlayers();
        return (
            <div>
                <PlayHeader text="Turn cycle"/>
                <div style={Styles.turnCycleSection}>
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

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game
    };
};

const TurnCycleSection = connect(mapStateToProps)(turnCycleSection);

export default TurnCycleSection;