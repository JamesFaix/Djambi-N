import * as React from 'react';
import { Game, User, CreatePlayerRequest, GameStatus } from '../../api/model';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';
import LobbyPlayersTableRow from './lobbyPlayersTableRow';

interface LobbyPlayersTableProps {
    user : User,
    game : Game,
    addPlayer : (gameId: number, request: CreatePlayerRequest) => void,
    removePlayer : (gameId: number, playerId: number) => void
}

class lobbyPlayersTable extends React.Component<LobbyPlayersTableProps> {
    render() {
        if (!this.props.game) {
            return null;
        }

        const seats = LobbySeats.getSeats(this.props.game, this.props.user);
        return (
            <div>
                Seats
                <table>
                    <tbody>
                        <tr>
                            <th>Player</th>
                            <th>Note</th>
                            {this.renderIfPending(<th>Action</th>)}
                        </tr>
                        {seats.map((s, i) => {
                            return (<LobbyPlayersTableRow
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
            </div>
        );
    }

    private renderIfPending(element : JSX.Element) : JSX.Element {
        return this.props.game.status === GameStatus.Pending ? element : null;
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game,
        user: state.session.user
    }
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        addPlayer: (gameId: number, request: CreatePlayerRequest) => ThunkActions.addPlayer(gameId, request)(dispatch),
        removePlayer: (gameId: number, playerId: number) => ThunkActions.removePlayer(gameId, playerId)(dispatch)
    }
};

const LobbyPlayersTable = connect(mapStateToProps, mapDispatchToProps)(lobbyPlayersTable);

export default LobbyPlayersTable;