import * as React from 'react';
import { LobbyWithPlayersResponse, PlayerResponse, UserResponse, PlayerKind, CreatePlayerRequest } from '../api/model';
import ActionButton from './actionButton';
import ApiClient from '../api/client';

export interface LobbyPlayersTableProps {
    api : ApiClient,
    user : UserResponse,
    lobby : LobbyWithPlayersResponse,
    updateLobby(lobby: LobbyWithPlayersResponse) : void
}

export interface LobbyPlayersTableState {
    guestName : string
}

enum SeatActionType {
    None,
    Remove,
    Join,
    AddGuest
}

interface Seat {
    player : PlayerResponse,
    note : string,
    action : SeatActionType
}

export default class LobbyPlayersTable extends React.Component<LobbyPlayersTableProps, LobbyPlayersTableState> {
    constructor(props : LobbyPlayersTableProps) {
        super(props);
        this.state = {
            guestName: ""
        };
    }

    private getSeats(lobby : LobbyWithPlayersResponse) : Seat[] {
        if (lobby === null) {
            return [];
        }

        const self = this.props.user;

        //Get player seats first
        const seats = lobby.players
            .map(p => {
                console.log(p);

                const seat : Seat = {
                    player : p,
                    note : null,
                    action : SeatActionType.None
                };

                if (p.kind === PlayerKind.User
                    && p.userId === lobby.createdByUserId) {
                    seat.note = "Host";
                }

                if (p.kind === PlayerKind.Guest) {
                    const host = lobby.players
                        .find(h => h.userId === p.userId
                            && h.kind === PlayerKind.User);

                    seat.note = "Guest of " + host.name;
                }

                if (self.isAdmin
                    || lobby.createdByUserId === self.id
                    || seat.player.name === self.name
                    || (seat.player.kind === PlayerKind.Guest
                        && seat.player.userId === self.id)) {

                    seat.action = SeatActionType.Remove;
                }
                console.log(seat);
                return seat;
            });

        //If self is not a player, add "Join" seat
        if (!this.isSelfAPlayer(lobby)) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.Join
            })
        //If self is a player and guests allowed, add "Add Guest" seat
        } else if (lobby.allowGuests) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.AddGuest
            });
        }

        //Add empty seats until regionCount
        while (seats.length < lobby.regionCount) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.None
            });
        }

        return seats;
    }

    private isSelfAPlayer(lobby : LobbyWithPlayersResponse) : boolean {
        return lobby.players.find(p => p.userId === this.props.user.id) !== undefined;
    }

    private isSeatSelf(seat : Seat) : boolean {
        return seat.player.kind === PlayerKind.User
            && seat.player.userId === this.props.user.id;
    }

    //---Event handlers---

    private addSelfOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: null,
            type: PlayerKind.User
        };

        this.props.api
            .addPlayer(this.props.lobby.id, request)
            .then(newPlayer => {
                const oldLobby = this.props.lobby;
                const newLobby : LobbyWithPlayersResponse = {
                    id: oldLobby.id,
                    regionCount: oldLobby.regionCount,
                    description: oldLobby.description,
                    allowGuests: oldLobby.allowGuests,
                    isPublic: oldLobby.isPublic,
                    createdByUserId: oldLobby.createdByUserId,
                    createdOn: oldLobby.createdOn,
                    players: oldLobby.players.concat(newPlayer)
                };

                this.props.updateLobby(newLobby);
            })
            .catch(reason => {
                alert("Add player failed because " + reason);
            });
    }

    private addGuestOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: this.state.guestName,
            type: PlayerKind.Guest
        };

        this.props.api
            .addPlayer(this.props.lobby.id, request)
            .then(newPlayer => {
                const oldLobby = this.props.lobby;
                const newLobby : LobbyWithPlayersResponse = {
                    id: oldLobby.id,
                    regionCount: oldLobby.regionCount,
                    description: oldLobby.description,
                    allowGuests: oldLobby.allowGuests,
                    isPublic: oldLobby.isPublic,
                    createdByUserId: oldLobby.createdByUserId,
                    createdOn: oldLobby.createdOn,
                    players: oldLobby.players.concat(newPlayer)
                };

                this.setState({
                        guestName : ""
                    },
                    () => this.props.updateLobby(newLobby));
            })
            .catch(reason => {
                alert("Add player failed because " + reason);
            });
    }

    private addGuestOnChanged(event : React.ChangeEvent<HTMLInputElement>) : void {
        const name = event.target.value;
        this.setState({ guestName: name });
    }

    private removeOnClick(playerId : number) : void {
        this.props.api
            .removePlayer(this.props.lobby.id, playerId)
            .then(_ => {
                const oldLobby = this.props.lobby;
                const removedPlayer = oldLobby.players.find(p => p.id === playerId);

                //Lobby is closed when creator is removed
                if (removedPlayer.kind === PlayerKind.User
                    && removedPlayer.userId === oldLobby.createdByUserId) {
                    this.props.updateLobby(null);
                }
                //Otherwise, remove player and all guests
                else {
                    const newLobby : LobbyWithPlayersResponse = {
                        id: oldLobby.id,
                        regionCount: oldLobby.regionCount,
                        description: oldLobby.description,
                        allowGuests: oldLobby.allowGuests,
                        isPublic: oldLobby.isPublic,
                        createdByUserId: oldLobby.createdByUserId,
                        createdOn: oldLobby.createdOn,
                        players: oldLobby.players.filter(p => p.userId !== removedPlayer.userId)
                    };

                    this.props.updateLobby(newLobby);
                }
            })
            .catch(reason => {
                alert("Remove player failed because " + reason);
            });
    }


    //---Rendering---

    private renderPlayerRow(seat : Seat, rowNumber : number) {
        switch (seat.action) {
            case SeatActionType.None:
                if (seat.player === null) {
                    return (
                        <tr key={"row" + rowNumber}>
                            <td className="lightText lobbyPlayersTableTextCell">
                                (Empty)
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    );
                } else {
                    return (
                        <tr key={"row" + rowNumber}>
                            <td className="lobbyPlayersTableTextCell">
                                {seat.player.name}
                            </td>
                            <td>{seat.note}</td>
                            <td></td>
                        </tr>
                    );
                }

            case SeatActionType.Join:
                return (
                    <tr key={"row" + rowNumber}>
                        <td className="lightText lobbyPlayersTableTextCell">
                            (Empty)
                        </td>
                        <td></td>
                        <td className="centeredContainer">
                            <ActionButton
                                label="Join"
                                onClick={() => this.addSelfOnClick()}
                            />
                        </td>
                    </tr>
                );

            case SeatActionType.AddGuest:
                return (
                    <tr key={"row" + rowNumber}>
                        <td>
                            <input
                                type="text"
                                value={this.state.guestName}
                                onChange={e => this.addGuestOnChanged(e)}
                                className="fullWidth"
                            />
                        </td>
                        <td></td>
                        <td className="centeredContainer">
                            <ActionButton
                                label="Add Guest"
                                onClick={() => this.addGuestOnClick()}
                            />
                        </td>
                    </tr>
                );

            case SeatActionType.Remove:
                const label = this.isSeatSelf(seat) ? "Quit" : "Remove";

                return (
                    <tr key={"row" + rowNumber}>
                        <td className="lobbyPlayersTableTextCell">
                            {seat.player.name}
                        </td>
                        <td>{seat.note}</td>
                        <td className="centeredContainer">
                            <ActionButton
                                label={label}
                                onClick={() => this.removeOnClick(seat.player.id)}
                            />
                        </td>
                    </tr>
                );

            default:
                return "";
        }
    }

    render() {
        if (this.props.lobby === null) {
            return "";
        }

        const seats = this.getSeats(this.props.lobby);

        return (
            <div>
                <table className="table">
                    <tbody>
                        <tr>
                            <td className="centeredContainer">
                                Seats
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table className="table">
                    <tbody>
                        {seats.map((seat, i) => this.renderPlayerRow(seat, i))}
                    </tbody>
                </table>
            </div>
        );
    }
}