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

export default class LobbyPlayersTableRow extends React.Component<LobbyPlayersTableRowProps> {
    render() {
        const seat = this.props.seat;

        const style : React.CSSProperties = {};

        if (seat.player && seat.player.colorId) {
            const color = Colors.getColorFromPlayerColorId(seat.player.colorId);
            style.boxShadow = `inset 0 0 0 3px ${color}`;
        }

        switch (seat.action) {
            case LobbySeats.SeatActionType.None:
                return (
                    <LobbyNoneRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        seat={seat}
                    />
                );

            case LobbySeats.SeatActionType.Join:
                return (
                    <LobbyJoinRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        addPlayer={this.props.addPlayer}
                    />
                );

            case LobbySeats.SeatActionType.AddGuest:
                return (
                    <LobbyAddGuestRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        addPlayer={this.props.addPlayer}
                    />
                );

            case LobbySeats.SeatActionType.Remove:
                return (
                    <LobbyRemoveRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        seat={seat}
                        removePlayer={this.props.removePlayer}
                    />
                );

            default:
                throw "Invalid SeatActionType";
        }
    }
}

interface LobbyRowProps {
    style : React.CSSProperties,
    game : Game,
    user : User
}

interface LobbyJoinRowProps extends LobbyRowProps {
    addPlayer : (gameId : number, request : CreatePlayerRequest) => void
}

class LobbyJoinRow extends React.Component<LobbyJoinRowProps> {
    render() {
        return (
            <tr style={this.props.style}>
                <td>(Empty)</td>
                <td></td>
                {this.props.game.status === GameStatus.Pending ?
                    <td>
                        <button
                            onClick={() => this.onClick()}
                        >
                            Join
                        </button>
                    </td>
                : null}
            </tr>
        );
    }

    private onClick() {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: null,
            kind: PlayerKind.User
        };
        this.props.addPlayer(this.props.game.id, request);
    }
}

interface LobbyAddGuestRowProps extends LobbyRowProps {
    addPlayer : (gameId : number, request : CreatePlayerRequest) => void
}

interface LobbyAddGuestRowState {
    guestName : string
}

class LobbyAddGuestRow extends React.Component<LobbyAddGuestRowProps, LobbyAddGuestRowState> {
    constructor(props: LobbyAddGuestRowProps) {
        super(props);
        this.state = {
            guestName: ""
        };
    }

    render() {
        return (
            <tr style={this.props.style}>
                <td>
                    <input
                        type="text"
                        value={this.state.guestName}
                        onChange={e => this.setState({guestName: e.target.value})}
                    />
                </td>
                <td></td>
                {this.props.game.status === GameStatus.Pending ?
                    <td>
                        <button
                            onClick={() => this.onClick()}
                        >
                            Add guest
                        </button>
                    </td>
                : null}
            </tr>
        );
    }

    private onClick() {
        this.setState({guestName: ""});

        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: this.state.guestName,
            kind: PlayerKind.Guest
        };
        this.props.addPlayer(this.props.game.id, request);
    }
}

interface LobbyRemoveRowProps extends LobbyRowProps {
    seat : Seat,
    removePlayer : (gameId : number, playerId : number) => void
}

class LobbyRemoveRow extends React.Component<LobbyRemoveRowProps> {
    render() {
        const seat = this.props.seat;
        return (
            <tr style={this.props.style}>
                <td>{seat.player.name}</td>
                <td>{seat.note}</td>
                {this.props.game.status === GameStatus.Pending ?
                    <td>
                        <button
                            onClick={() => this.props.removePlayer(this.props.game.id, seat.player.id)}
                        >
                            {LobbySeats.isSeatSelf(seat, this.props.user) ? "Quit" : "Remove"}
                        </button>
                    </td>
                : null}
            </tr>
        );
    }
}

interface LobbyNoneRowProps extends LobbyRowProps {
    seat : Seat
}

class LobbyNoneRow extends React.Component<LobbyNoneRowProps> {
    render() {
        const seat = this.props.seat;

        const playerName = seat.player
            ? seat.player.name
            : "(Empty)";

        return (
            <tr style={this.props.style}>
                <td>{playerName}</td>
                <td>{seat.note}</td>
                {this.props.game.status === GameStatus.Pending ?
                    <td></td>
                : null}
            </tr>
        );
    }
}