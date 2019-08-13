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

        const isPending = this.props.game.status === GameStatus.Pending;

        switch (seat.action) {
            case LobbySeats.SeatActionType.None:
                return (
                    <LobbyNoneRow
                        style={style}
                        isGamePending={isPending}
                        seat={seat}
                    />
                );

            case LobbySeats.SeatActionType.Join:
                return (
                    <LobbyJoinRow
                        style={style}
                        isGamePending={isPending}
                        onAddSelfClicked={() => {
                            const request : CreatePlayerRequest = {
                                userId: this.props.currentUser.id,
                                name: null,
                                kind: PlayerKind.User
                            };
                            this.props.addPlayer(this.props.game.id, request);
                        }}
                    />
                );

            case LobbySeats.SeatActionType.AddGuest:
                return (
                    <LobbyAddGuestRow
                        style={style}
                        isGamePending={isPending}
                        onAddGuestClicked={(guestName: string) => {
                            const request : CreatePlayerRequest = {
                                userId: this.props.currentUser.id,
                                name: guestName,
                                kind: PlayerKind.Guest
                            };
                            this.props.addPlayer(this.props.game.id, request);
                        }}
                    />
                );

            case LobbySeats.SeatActionType.Remove:
                return (
                    <LobbyRemoveRow
                        style={style}
                        isGamePending={isPending}
                        seat={seat}
                        currentUser={this.props.currentUser}
                        onRemoveClicked={(playerId : number) =>
                            this.props.removePlayer(this.props.game.id, playerId)}
                    />
                );

            default:
                throw "Invalid SeatActionType";
        }
    }
}

interface LobbyJoinRowProps {
    style : React.CSSProperties,
    isGamePending : boolean,
    onAddSelfClicked : () => void
}

class LobbyJoinRow extends React.Component<LobbyJoinRowProps> {
    render() {
        return (
            <tr style={this.props.style}>
                <td>(Empty)</td>
                <td></td>
                {this.props.isGamePending ?
                    <td>
                        <button
                            onClick={() => this.props.onAddSelfClicked()}
                        >
                            Join
                        </button>
                    </td>
                : null}
            </tr>
        );
    }
}

interface LobbyAddGuestRowProps {
    style : React.CSSProperties,
    isGamePending : boolean,
    onAddGuestClicked : (name : string) => void
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
                {this.props.isGamePending ?
                    <td>
                        <button
                            onClick={() => {
                                this.setState({guestName: ""});
                                this.props.onAddGuestClicked(this.state.guestName);
                            }}
                        >
                            Add guest
                        </button>
                    </td>
                : null}
            </tr>
        );
    }
}

interface LobbyRemoveRowProps {
    style : React.CSSProperties,
    isGamePending : boolean,
    seat : Seat,
    currentUser : User,
    onRemoveClicked : (playerId : number) => void
}

class LobbyRemoveRow extends React.Component<LobbyRemoveRowProps> {
    render() {
        const seat = this.props.seat;
        return (
            <tr style={this.props.style}>
                <td>{seat.player.name}</td>
                <td>{seat.note}</td>
                {this.props.isGamePending ?
                    <td>
                        <button
                            onClick={() => this.props.onRemoveClicked(seat.player.id)}
                        >
                            {LobbySeats.isSeatSelf(seat, this.props.currentUser) ? "Quit" : "Remove"}
                        </button>
                    </td>
                : null}
            </tr>
        );
    }
}

interface LobbyNoneRowProps {
    style : React.CSSProperties,
    isGamePending : boolean,
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
                {this.props.isGamePending ?
                    <td></td>
                : null}
            </tr>
        );
    }
}