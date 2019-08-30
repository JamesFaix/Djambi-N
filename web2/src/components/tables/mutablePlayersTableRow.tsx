import * as React from 'react';
import { Seat } from '../../viewModel/lobbySeats';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { User, CreatePlayerRequest, PlayerKind, Game } from '../../api/model';
import IconButton from '../controls/iconButton';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import { Icons } from '../../utilities/icons';

interface MutablePlayersTableRowProps {
    game : Game,
    currentUser : User,
    seat : Seat,
    addPlayer : (gameId : number, request: CreatePlayerRequest) => void,
    removePlayer : (gameId : number, playerId : number) => void
}

export default class MutablePlayersTableRow extends React.Component<MutablePlayersTableRowProps> {
    render() {
        const seat = this.props.seat;

        switch (seat.action) {
            case LobbySeats.SeatActionType.None:
                return (
                    <NoActionRow
                        game={this.props.game}
                        user={this.props.currentUser}
                        seat={seat}
                    />
                );

            case LobbySeats.SeatActionType.Join:
                return (
                    <JoinRow
                        game={this.props.game}
                        user={this.props.currentUser}
                        addPlayer={this.props.addPlayer}
                    />
                );

            case LobbySeats.SeatActionType.AddGuest:
                return (
                    <AddGuestRow
                        game={this.props.game}
                        user={this.props.currentUser}
                        addPlayer={this.props.addPlayer}
                    />
                );

            case LobbySeats.SeatActionType.Remove:
                return (
                    <RemoveRow
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

interface RowPropsBase {
    game : Game,
    user : User
}

//#region Join

interface JoinRowProps extends RowPropsBase {
    addPlayer : (gameId : number, request : CreatePlayerRequest) => void
}

class JoinRow extends React.Component<JoinRowProps> {
    render() {
        return (
            <tr>
                <td>(Empty)</td>
                <td></td>
                <td>
                    <IconButton
                        icon={Icons.UserActions.playerJoin}
                        onClick={() => this.onClick()}
                    />
                </td>
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

//#endregion

//#region Add guest

interface AddGuestRowProps extends RowPropsBase {
    addPlayer : (gameId : number, request : CreatePlayerRequest) => void
}

interface AddGuestRowState {
    guestName : string
}

class AddGuestRow extends React.Component<AddGuestRowProps, AddGuestRowState> {
    constructor(props: AddGuestRowProps) {
        super(props);
        this.state = {
            guestName: ""
        };
    }

    render() {
        return (
            <tr>
                <td>
                    <input
                        type="text"
                        value={this.state.guestName}
                        onChange={e => this.setState({guestName: e.target.value})}
                    />
                </td>
                <td></td>
                <td>
                    <IconButton
                        icon={Icons.UserActions.playerAddGuest}
                        onClick={() => this.onClick()}
                    />
                </td>
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

//#endergion

//#region Remove

interface RemoveRowProps extends RowPropsBase {
    seat : Seat,
    removePlayer : (gameId : number, playerId : number) => void
}

class RemoveRow extends React.Component<RemoveRowProps> {
    render() {
        const seat = this.props.seat;
        const icon = LobbySeats.isSeatSelf(seat, this.props.user)
            ? Icons.UserActions.playerQuit
            : Icons.UserActions.playerRemove;

        return (
            <tr>
                <td>{seat.player.name}</td>
                <td>
                    <PlayerNoteIcon
                        player={seat.player}
                        game={this.props.game}
                    />
                </td>
                <td>
                    <IconButton
                        icon={icon}
                        onClick={() => this.props.removePlayer(this.props.game.id, seat.player.id)}
                    />
                </td>
            </tr>
        );
    }
}

//#endregion

//#region None

interface NoActionRowProps extends RowPropsBase {
    seat : Seat
}

class NoActionRow extends React.Component<NoActionRowProps> {
    render() {
        const seat = this.props.seat;

        const playerName = seat.player
            ? seat.player.name
            : "(Empty)";

        return (
            <tr>
                <td>{playerName}</td>
                <td>
                    <PlayerNoteIcon
                        player={seat.player}
                        game={this.props.game}
                    />
                </td>
                <td></td>
            </tr>
        );
    }
}

//#endregion