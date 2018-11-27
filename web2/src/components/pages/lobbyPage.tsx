import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyWithPlayersResponse, PlayerResponse, PlayerType } from '../../api/model';
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
    lobbyWithPlayers : LobbyWithPlayersResponse
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
            lobbyWithPlayers : null
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

    private removeOnClicked(playerId : number) : void {
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
                    onClick={() => this.removeOnClicked(player.id)}
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
            </div>
        );
    }
}