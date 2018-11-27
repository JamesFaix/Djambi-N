import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyWithPlayersResponse, PlayerResponse, PlayerType, CreatePlayerRequest } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';
import LabeledInput from '../labeledInput';

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

interface LobbyPlayerViewModel {
    id : number,
    name : string,
    guestOf : string,
    canRemove : boolean
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

    private getPlayerViewModels(lobby : LobbyWithPlayersResponse) : LobbyPlayerViewModel[] {
        if (lobby === null) {
            return [];
        }

        const self = this.props.user;

        return lobby.players
            .map(p => {
                const vm : LobbyPlayerViewModel = {
                    id : p.id,
                    name : p.name,
                    guestOf : null,
                    canRemove : false
                };

                if (p.type === PlayerType.Guest) {
                    const host = lobby.players
                        .find(h => h.userId === p.userId
                            && h.type === PlayerType.User);

                    vm.guestOf = host.name;
                }

                vm.canRemove = self.isAdmin
                    || lobby.createdByUserId === self.id
                    || vm.name === self.name
                    || vm.guestOf === self.name;

                return vm;
            });
    }

    private isSelfAPlayer(lobby : LobbyWithPlayersResponse) : boolean {
        return lobby.players.find(p => p.userId === this.props.user.id) !== undefined;
    }

//---Event handlers---

    private addSelfOnClick() : void {
        const request = new CreatePlayerRequest(
            this.props.user.id,
            null,
            PlayerType.User);

        this.props.api
            .addPlayer(this.props.lobbyId, request)
            .then(newPlayer => {
                const oldLobby = this.state.lobbyWithPlayers;
                const newLobby = new LobbyWithPlayersResponse(
                    oldLobby.id,
                    oldLobby.regionCount,
                    oldLobby.description,
                    oldLobby.allowGuests,
                    oldLobby.isPublic,
                    oldLobby.createdByUserId,
                    oldLobby.players.concat(newPlayer));

                this.setState({
                    lobbyWithPlayers : newLobby
                });
            })
            .catch(reason => {
                alert("Add player failed because " + reason);
            });
    }

    private addGuestOnClick() : void {
        const request = new CreatePlayerRequest(
            this.props.user.id,
            this.state.guestName,
            PlayerType.Guest);

        this.props.api
            .addPlayer(this.props.lobbyId, request)
            .then(newPlayer => {
                const oldLobby = this.state.lobbyWithPlayers;
                const newLobby = new LobbyWithPlayersResponse(
                    oldLobby.id,
                    oldLobby.regionCount,
                    oldLobby.description,
                    oldLobby.allowGuests,
                    oldLobby.isPublic,
                    oldLobby.createdByUserId,
                    oldLobby.players.concat(newPlayer));

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
        const input = event.target;
        switch (input.name) {
            case "Guest name":
                this.setState({ guestName: input.value });
                break;

            default:
                break;
        }
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
                    const newLobby = new LobbyWithPlayersResponse(
                        oldLobby.id,
                        oldLobby.regionCount,
                        oldLobby.description,
                        oldLobby.allowGuests,
                        oldLobby.isPublic,
                        oldLobby.createdByUserId,
                        oldLobby.players.filter(p => p.userId !== removedPlayer.userId));

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
                    <p>{lobby.players.length + " of " + lobby.regionCount + " seats taken"}</p>
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

    private renderRemoveButton(player : LobbyPlayerViewModel) {
        if (player.canRemove){
            return (
                <ActionButton
                    label="Remove"
                    onClick={() => this.removeOnClick(player.id)}
                />
                );
        } else {
            return "";
        }
    }

    private renderPlayersTable(lobby : LobbyWithPlayersResponse) {
        if (lobby === null) {
            return "";
        }

        const viewModels = this.getPlayerViewModels(lobby);

        return (
            <div className="centeredContainer">
                <table className="lobbyPlayersTable">
                    <tbody>
                        <tr>
                            <th className="lobbyPlayersTableHeader">Name</th>
                            <th className="lobbyPlayersTableHeader">Guest of</th>
                            <th className="lobbyPlayersTableHeader">(Remove)</th>
                        </tr>
                        {viewModels.map((vm, i) =>
                            <tr key={"row" + i}>
                                <td>{vm.name}</td>
                                <td>{vm.guestOf}</td>
                                <td>{this.renderRemoveButton(vm)}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    private renderAddSelf(lobby : LobbyWithPlayersResponse) {
        if (lobby === null) {
            return "";
        }

        if (!lobby.allowGuests
            || lobby.regionCount === lobby.players.length
            || !this.isSelfAPlayer(lobby)) {
            return "";
        }

        return (
            <div className="centeredContainer addGuestContainer">
                <table>
                    <tbody>
                        <tr>
                            <td className="addGuestTableCell">
                                <LabeledInput
                                    type="text"
                                    value={this.state.guestName}
                                    label="Guest name"
                                    handleChange={e => this.addGuestOnChanged(e)}
                                />
                            </td>
                            <td className="addGuestTableCell">
                                <ActionButton
                                    label="Add"
                                    onClick={() => this.addGuestOnClick()}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }

    private renderAddGuest(lobby : LobbyWithPlayersResponse) {
        if (lobby === null) {
            return "";
        }

        if (lobby.regionCount === lobby.players.length
            || this.isSelfAPlayer(lobby)) {
            return "";
        }

        return (
            <div className="centeredContainer">
                <ActionButton
                    label="Join"
                    onClick={() => this.addSelfOnClick()}
                />
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
                <br/>
                {this.renderPlayersTable(this.state.lobbyWithPlayers)}
                <br/>
                {this.renderAddSelf(this.state.lobbyWithPlayers)}
                {this.renderAddGuest(this.state.lobbyWithPlayers)}
            </div>
        );
    }
}