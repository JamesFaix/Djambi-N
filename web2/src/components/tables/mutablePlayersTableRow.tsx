import * as React from 'react';
import { Seat } from '../../viewModel/lobbySeats';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { User, CreatePlayerRequest, PlayerKind, Game, GameStatus } from '../../api/model';
import Colors from '../../utilities/colors';
import IconButton from '../controls/iconButton';
import Icons from '../../utilities/icons';
import PlayerNoteIcon from '../controls/playerNoteIcon';

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

        const style : React.CSSProperties = {};

        if (seat.player && seat.player.colorId) {
            const color = Colors.getColorFromPlayerColorId(seat.player.colorId);
            style.boxShadow = `inset 0 0 0 3px ${color}`;
        }

        switch (seat.action) {
            case LobbySeats.SeatActionType.None:
                return (
                    <NoActionRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        seat={seat}
                    />
                );

            case LobbySeats.SeatActionType.Join:
                return (
                    <JoinRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        addPlayer={this.props.addPlayer}
                    />
                );

            case LobbySeats.SeatActionType.AddGuest:
                return (
                    <AddGuestRow
                        style={style}
                        game={this.props.game}
                        user={this.props.currentUser}
                        addPlayer={this.props.addPlayer}
                    />
                );

            case LobbySeats.SeatActionType.Remove:
                return (
                    <RemoveRow
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

interface RowPropsBase {
    style : React.CSSProperties,
    game : Game,
    user : User
}

const cellStyle = {
    borderStyle: "none"
}

//#region Join

interface JoinRowProps extends RowPropsBase {
    addPlayer : (gameId : number, request : CreatePlayerRequest) => void
}

class JoinRow extends React.Component<JoinRowProps> {
    render() {
        return (
            <tr style={this.props.style}>
                <td style={cellStyle}>(Empty)</td>
                <td style={cellStyle}></td>
                {this.props.game.status === GameStatus.Pending ?
                    <td style={cellStyle}>
                        <IconButton
                            title="Join"
                            icon={Icons.join}
                            onClick={() => this.onClick()}
                        />
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
            <tr style={this.props.style}>
                <td style={cellStyle}>
                    <input
                        type="text"
                        value={this.state.guestName}
                        onChange={e => this.setState({guestName: e.target.value})}
                    />
                </td>
                <td style={cellStyle}></td>
                {this.props.game.status === GameStatus.Pending ?
                    <td style={cellStyle}>
                        <IconButton
                            title="Add guest"
                            icon={Icons.addGuest}
                            onClick={() => this.onClick()}
                        />
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

//#endergion

//#region Remove

interface RemoveRowProps extends RowPropsBase {
    game : Game,
    seat : Seat,
    removePlayer : (gameId : number, playerId : number) => void
}

class RemoveRow extends React.Component<RemoveRowProps> {
    render() {
        const seat = this.props.seat;
        return (
            <tr style={this.props.style}>
                <td style={cellStyle}>{seat.player.name}</td>
                <td style={cellStyle}>
                    <PlayerNoteIcon
                        player={seat.player}
                        game={this.props.game}
                    />
                </td>
                {this.props.game.status === GameStatus.Pending ?
                    <td style={cellStyle}>
                        <IconButton
                            title={LobbySeats.isSeatSelf(seat, this.props.user) ? "Quit" : "Remove"}
                            icon={Icons.quit}
                            onClick={() => this.props.removePlayer(this.props.game.id, seat.player.id)}
                        />
                    </td>
                : null}
            </tr>
        );
    }
}

//#endregion

//#region None

interface NoActionRowProps extends RowPropsBase {
    seat : Seat,
    game : Game
}

class NoActionRow extends React.Component<NoActionRowProps> {
    render() {
        const seat = this.props.seat;

        const playerName = seat.player
            ? seat.player.name
            : "(Empty)";

        return (
            <tr style={this.props.style}>
                <td style={cellStyle}>{playerName}</td>
                <td style={cellStyle}>
                    <PlayerNoteIcon
                        player={seat.player}
                        game={this.props.game}
                    />
                </td>
                {this.props.game.status === GameStatus.Pending ?
                    <td style={cellStyle}></td>
                : null}
            </tr>
        );
    }
}

//#endregion