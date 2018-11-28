import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyWithPlayersResponse, PlayerResponse, PlayerType, CreatePlayerRequest } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';

export interface LobbyPageProps {
    user : UserResponse,
    api : ApiClient,
    lobbyId : number,
    setLobbyId(lobbyId : number) : void
}

export interface LobbyPageState {
    lobbyWithPlayers : LobbyWithPlayersResponse,
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

export default class LobbyPage extends React.Component<LobbyPageProps, LobbyPageState> {

    constructor(props : LobbyPageProps) {
        super(props);
        this.state = {
            lobbyWithPlayers : null,
            guestName : ""
        };
    }

    componentDidMount() {
        //Set lobbyId in state upon navigation
        this.props.setLobbyId(this.props.lobbyId);

        this.props.api
            .getLobby(this.props.lobbyId)
            .then(lobbyWithPlayers => {
                this.setState({lobbyWithPlayers : lobbyWithPlayers});
            })
            .catch(reason => {
                alert("Get lobby failed because " + reason);
            });
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

                if (p.type === PlayerType.User
                    && p.userId === lobby.createdByUserId) {
                    seat.note = "Host";
                }

                if (p.type === PlayerType.Guest) {
                    const host = lobby.players
                        .find(h => h.userId === p.userId
                            && h.type === PlayerType.User);

                    seat.note = "Guest of " + host.name;
                }

                if (self.isAdmin
                    || lobby.createdByUserId === self.id
                    || seat.player.name === self.name
                    || (seat.player.type === PlayerType.Guest
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
        return seat.player.type === PlayerType.User
            && seat.player.userId === this.props.user.id;
    }

//---Event handlers---

    private addSelfOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: null,
            type: PlayerType.User
        };

        this.props.api
            .addPlayer(this.props.lobbyId, request)
            .then(newPlayer => {
                const oldLobby = this.state.lobbyWithPlayers;
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
                    lobbyWithPlayers : newLobby
                });
            })
            .catch(reason => {
                alert("Add player failed because " + reason);
            });
    }

    private addGuestOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: this.state.guestName,
            type: PlayerType.Guest
        };

        this.props.api
            .addPlayer(this.props.lobbyId, request)
            .then(newPlayer => {
                const oldLobby = this.state.lobbyWithPlayers;
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
                    lobbyWithPlayers : newLobby,
                    guestName : ""
                });
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
            .removePlayer(this.props.lobbyId, playerId)
            .then(_ => {
                const oldLobby = this.state.lobbyWithPlayers;
                const removedPlayer = oldLobby.players.find(p => p.id === playerId);

                //Lobby is closed when creator is removed
                if (removedPlayer.type === PlayerType.User
                    && removedPlayer.userId === oldLobby.createdByUserId) {
                    this.props.setLobbyId(null);
                    this.setState({lobbyWithPlayers : null});
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

                    this.setState({lobbyWithPlayers : newLobby});
                }
            })
            .catch(reason => {
                alert("Remove player failed because " + reason);
            });
    }

//---Rendering---

    private renderLobbyDetails(lobby : LobbyWithPlayersResponse) {
        if (lobby === null){
            return "";
        }

        return (
            <div className="lobbyDetailsContainer">
                <div className="centeredContainer">
                    {this.renderLobbyDescription(lobby)}
                    {this.renderLobbyOptions(lobby)}
                    <p>{lobby.regionCount + " regions"}</p>
                </div>
            </div>
        );
    }

    private renderLobbyDescription(lobby : LobbyWithPlayersResponse) {
        if (lobby.description === null) {
            return "";
        } else {
            return <p>{lobby.description}</p>;
        }
    }

    private renderLobbyOptions(lobby : LobbyWithPlayersResponse) {
        if (lobby.isPublic && lobby.allowGuests) {
            return <p>{"Public, guests allowed"}</p>;
        } else if (lobby.isPublic) {
            return <p>{"Public"}</p>;
        } else if (lobby.allowGuests) {
            return <p>{"Guests allowed"}</p>;
        } else {
            return "";
        }
    }

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

    private renderPlayersTable(lobby : LobbyWithPlayersResponse) {
        if (lobby === null) {
            return "";
        }

        const seats = this.getSeats(lobby);

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

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>;
        }

        //Go to home if lobby has 0 players (lobby closed)
        if (this.state.lobbyWithPlayers !== null
            && this.state.lobbyWithPlayers.players.length === 0) {
            return <Redirect to='/'/>;
        }

        return (
            <div>
                <PageTitle label={"Lobby"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="My Games" to="/myGames"/>
                    <LinkButton label="Create Game" to="/createGame"/>
                    <LinkButton label="Find Game" to="/findGame"/>
                </div>
                {this.renderLobbyDetails(this.state.lobbyWithPlayers)}
                {this.renderPlayersTable(this.state.lobbyWithPlayers)}
            </div>
        );
    }
}