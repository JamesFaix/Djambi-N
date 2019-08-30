import * as React from 'react';
import { Game, Player } from '../../api/model';
import TurnCycleTurnBox from './turnCycleTurnBox';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { TimelineHeader } from '../controls/headers';
import { Styles } from '../../styles/styles';
import { Icons } from '../../utilities/icons';

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
                <TimelineHeader icon={Icons.Timeline.turnCycle}/>
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