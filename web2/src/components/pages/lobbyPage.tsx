import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse, LobbyWithPlayersResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';
import LobbyPlayersTable from '../lobbyPlayersTable';

export interface LobbyPageProps {
    user : UserResponse,
    api : ApiClient,
    lobbyId : number
}

export interface LobbyPageState {
    lobby : LobbyWithPlayersResponse,
    guestName : string,
    lobbyId : number
}

export default class LobbyPage extends React.Component<LobbyPageProps, LobbyPageState> {

    constructor(props : LobbyPageProps) {
        super(props);
        this.state = {
            lobby : null,
            guestName : "",
            lobbyId : props.lobbyId
        };
    }

    componentDidMount() {
        this.props.api
            .getLobby(this.props.lobbyId)
            .then(lobbyWithPlayers => {
                this.setState({lobby : lobbyWithPlayers});
            })
            .catch(reason => {
                alert("Get lobby failed because " + reason);
            });
    }

//---Event handlers---

    private playerTableUpdateLobby(newLobby : LobbyWithPlayersResponse) {
        if (newLobby === null) {
            this.setState({lobbyId : null});
        } else {
            this.setState({lobby : newLobby});
        }
    }

    private startOnClick() {
        this.props.api
            .startGame(this.state.lobby.id)
            .then(gameStart => {
                alert("Game started");
            })
            .catch(reason => {
                alert("Game start failed because " + reason);
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

    private renderStartButton(lobby: LobbyWithPlayersResponse){
        if (lobby === null) {
            return "";
        }

        //Only creator can start
        if (this.props.user.id !== lobby.createdByUserId) {
            return "";
        }

        //Can only start with at least 2 players
        if (lobby.players.length < 2){
            return "";
        }

        return (
            <div className="centeredContainer">
                <ActionButton
                    label="Start"
                    onClick={() => this.startOnClick()}
                />
            </div>
        );
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>;
        }

        //LobbyId is set to null when lobby closed
        if (this.state.lobbyId === null) {
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
                {this.renderLobbyDetails(this.state.lobby)}
                <LobbyPlayersTable
                    user={this.props.user}
                    api={this.props.api}
                    lobby={this.state.lobby}
                    updateLobby={newLobby => this.playerTableUpdateLobby(newLobby)}
                />
                <br/>
                {this.renderStartButton(this.state.lobby)}
            </div>
        );
    }
}