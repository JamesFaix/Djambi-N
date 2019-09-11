import * as React from 'react';
import { Game, User, CreatePlayerRequest } from '../../api/model';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import MutablePlayersTableRow from './mutablePlayersTableRow';
import { SectionHeader } from '../controls/headers';
import Controller from '../../controller';

interface MutablePlayersTableProps {
    user : User,
    game : Game,
    addPlayer : (gameId: number, request: CreatePlayerRequest) => void,
    removePlayer : (gameId: number, playerId: number) => void
}

class mutablePlayersTable extends React.Component<MutablePlayersTableProps> {
    render() {
        if (!this.props.game) {
            return null;
        }

        const seats = LobbySeats.getSeats(this.props.game, this.props.user);
        return (<>
            <SectionHeader text="Players"/>
            <table>
                <tbody>
                    {seats.map((s, i) => {
                        return (<MutablePlayersTableRow
                            game={this.props.game}
                            currentUser={this.props.user}
                            seat={s}
                            addPlayer={this.props.addPlayer}
                            removePlayer={this.props.removePlayer}
                            key={i}
                        />);
                    })}
                </tbody>
            </table>
        </>);
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game,
        user: state.session.user,
        addPlayer: (gameId: number, request: CreatePlayerRequest) => Controller.Game.addPlayer(gameId, request),
        removePlayer: (gameId: number, playerId: number) => Controller.Game.removePlayer(gameId, playerId)
    }
};

const MutablePlayersTable = connect(mapStateToProps)(mutablePlayersTable);

export default MutablePlayersTable;