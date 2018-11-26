import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyWithPlayersResponse, PlayerResponse, PlayerType } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';

export interface LobbyPageProps {
    user : UserResponse,
    api : ApiClient,
    lobbyId : number,
    setLobbyId(lobbyId : number) : void
}

export interface LobbyPageState {
    lobbyWithPlayers : LobbyWithPlayersResponse
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

    private renderLobbyDetails(lobby : LobbyWithPlayersResponse) {
        if (lobby === null){
            return "";
        }

        return (
            <div className="lobbyDetailsContainer">
                <div className="centeredContainer">
                    {this.renderLobbyDescription(lobby)}
                    <p>{lobby.regionCount + " regions"}</p>
                    {this.renderLobbyOptions(lobby)}
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

    private renderPlayersTable(lobby : LobbyWithPlayersResponse) {
        if (lobby === null) {
            return "";
        }

        const playersWithHostName = lobby.players
            .map(p => {
                if (p.type === PlayerType.Guest){
                    const host = lobby.players
                        .find(h => h.userId === p.userId
                            && h.type === PlayerType.User);

                    return {
                        name : p.name,
                        hostName : host.name
                    };
                } else {
                    return {
                        name : p.name,
                        hostName : null
                    };
                }
            });

        return (
            <div className="centeredContainer">
                <table className="lobbyPlayersTable">
                    <tbody>
                        <tr>
                            <th className="lobbyPlayersTableHeader">Name</th>
                            <th className="lobbyPlayersTableHeader">Guest of</th>
                        </tr>
                        {playersWithHostName.map((p, i) =>
                            <tr key={"row" + i}>
                                <td>{p.name}</td>
                                <td>{p.hostName}</td>
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
            return <Redirect to='/'/>
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