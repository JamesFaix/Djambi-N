import * as React from 'react';
import { Seat } from '../../viewModel/lobbySeats';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { User, CreatePlayerRequest, PlayerKind, Game, GameStatus } from '../../api/model';
import Colors from '../../utilities/colors';

interface LobbyPlayersTableRowProps {
    game : Game,
    currentUser : User,
    seat : Seat,
    addPlayer : (gameId : number, request: CreatePlayerRequest) => void,
    removePlayer : (gameId : number, playerId : number) => void
}

interface LobbyPlayersTableRowState {
    guestName : string
}

export default class LobbyPlayersTableRow extends React.Component<LobbyPlayersTableRowProps, LobbyPlayersTableRowState> {
    constructor(props : LobbyPlayersTableRowProps) {
        super(props);
        this.state = {
            guestName : ""
        };
    }

    render() {
        const seat = this.props.seat;

        const style : React.CSSProperties = {};

        if (seat.player.colorId) {
            const color = Colors.getColorFromPlayerColorId(seat.player.colorId);
            style.boxShadow = `inset 0 0 0 3px ${color}`;
        }

        switch (seat.action) {
            case LobbySeats.SeatActionType.None:
                const playerName = seat.player
                    ? seat.player.name
                    : "(Empty)";

                return (
                    <tr style={style}>
                        <td>{playerName}</td>
                        <td>{seat.note}</td>
                        {this.renderIfPending(<td></td>)}
                    </tr>
                );

            case LobbySeats.SeatActionType.Join:
                return (
                    <tr style={style}>
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
                    <tr style={style}>
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
                    <tr style={style}>
                        <td>{seat.player.name}</td>
                        <td>{seat.note}</td>
                        {this.renderIfPending(
                            <td>
                                <button
                                    onClick={() => this.removeOnClick(seat.player.id)}
                                >
                                    {LobbySeats.isSeatSelf(seat, this.props.currentUser) ? "Quit" : "Remove"}
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
            userId: this.props.currentUser.id,
            name: null,
            kind: PlayerKind.User
        };

        this.props.addPlayer(this.props.game.id, request);
    }

    private addGuestOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.currentUser.id,
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