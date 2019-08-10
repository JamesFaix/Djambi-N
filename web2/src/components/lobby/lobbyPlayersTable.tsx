import * as React from 'react';
import { Game, User, CreatePlayerRequest, GameStatus, PlayerKind } from '../../api/model';
import * as LobbySeats from './lobbySeats';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import * as ThunkActions from '../../thunkActions';

interface LobbyPlayersTableProps {
    user : User,
    game : Game,
    addPlayer : (gameId: number, request: CreatePlayerRequest) => void,
    removePlayer : (gameId: number, playerId: number) => void
}

interface LobbyPlayersTableState {
    guestName : string
}

class lobbyPlayersTable extends React.Component<LobbyPlayersTableProps, LobbyPlayersTableState> {
    constructor(props : LobbyPlayersTableProps) {
        super(props);
        this.state = {
            guestName: ""
        };
    }

    render() {
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
                        {seats.map((s, i) => this.renderRow(s, i))}
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(seat : LobbySeats.Seat, rowNumber : number) {
        switch (seat.action) {
            case LobbySeats.SeatActionType.None:
                const playerName = seat.player
                    ? seat.player.name
                    : "(Empty)";

                return (
                    <tr key={"row" + rowNumber}>
                        <td>{playerName}</td>
                        <td>{seat.note}</td>
                        {this.renderIfPending(<td></td>)}
                    </tr>
                );

            case LobbySeats.SeatActionType.Join:
                return (
                    <tr key={"row" + rowNumber}>
                        <td>(Empty)</td>
                        <td></td>
                        {this.renderIfPending(
                            <td>
                                <button
                                    onClick={() => this.addSelfOnClick()}
                                >
                                    Join
                                </button>
                            </td>
                        )}
                    </tr>
                );

            case LobbySeats.SeatActionType.AddGuest:
                return (
                    <tr key={"row" + rowNumber}>
                        <td>
                            <input
                                type="text"
                                value={this.state.guestName}
                                onChange={e => this.addGuestOnChanged(e)}
                            />
                        </td>
                        <td></td>
                        {this.renderIfPending(
                            <td>
                                <button
                                    onClick={() => this.addGuestOnClick()}
                                >
                                    Add guest
                                </button>
                            </td>
                        )}
                    </tr>
                );

            case LobbySeats.SeatActionType.Remove:
                return (
                    <tr key={"row" + rowNumber}>
                        <td>{seat.player.name}</td>
                        <td>{seat.note}</td>
                        {this.renderIfPending(
                            <td>
                                <button
                                    onClick={() => this.removeOnClick(seat.player.id)}
                                >
                                    {LobbySeats.isSeatSelf(seat, this.props.user) ? "Quit" : "Remove"}
                                </button>
                            </td>
                        )}
                    </tr>
                );

            default:
                throw "Invalid SeatActionType";
        }
    }

    private renderIfPending(element : JSX.Element) : JSX.Element {
        return this.props.game.status === GameStatus.Pending ? element : null;
    }

    private addSelfOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: null,
            kind: PlayerKind.User
        };

        this.props.addPlayer(this.props.game.id, request);
    }

    private addGuestOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: this.state.guestName,
            kind: PlayerKind.Guest
        };

        this.props.addPlayer(this.props.game.id, request);
        this.setState({guestName: ""});
    }

    private addGuestOnChanged(event : React.ChangeEvent<HTMLInputElement>) : void {
        const name = event.target.value;
        this.setState({ guestName: name });
    }

    private removeOnClick(playerId : number) : void {
        this.props.removePlayer(this.props.game.id, playerId);
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