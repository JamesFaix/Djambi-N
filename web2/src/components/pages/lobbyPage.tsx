import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User, Game } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';
import GamePlayersTable from '../gamePlayersTable';

export interface LobbyPageProps {
    user : User,
    api : ApiClient,
    gameId : number
}

export interface LobbyPageState {
    game : Game,
    guestName : string,
    gameId : number
}

export default class LobbyPage extends React.Component<LobbyPageProps, LobbyPageState> {

    constructor(props : LobbyPageProps) {
        super(props);
        this.state = {
            game : null,
            guestName : "",
            gameId : props.gameId
        };
    }

    componentDidMount() {
        this.props.api
            .getGame(this.props.gameId)
            .then(game => {
                this.setState({game : game});
            })
            .catch(reason => {
                alert("Get lobby failed because " + reason);
            });
    }

//---Event handlers---

    private playerTableUpdateGame(newGame : Game) {
        if (newGame === null) {
            this.setState({gameId : null});
        } else {
            this.setState({game : newGame});
        }
    }

    private startOnClick() {
        this.props.api
            .startGame(this.state.game.id)
            .then(gameStart => {
                alert("Game started");
            })
            .catch(reason => {
                alert("Game start failed because " + reason);
            });
    }

//---Rendering---

    private renderLobbyDetails(game : Game) {
        if (game === null){
            return "";
        }

        return (
            <div className="lobbyDetailsContainer">
                <div className="centeredContainer">
                    {this.renderLobbyDescription(game)}
                    {this.renderLobbyOptions(game)}
                    <p>{game.parameters.regionCount + " regions"}</p>
                </div>
            </div>
        );
    }

    private renderLobbyDescription(game : Game) {
        if (game.parameters.description === null) {
            return "";
        } else {
            return <p>{game.parameters.description}</p>;
        }
    }

    private renderLobbyOptions(game : Game) {
        if (game.parameters.isPublic && game.parameters.allowGuests) {
            return <p>{"Public, guests allowed"}</p>;
        } else if (game.parameters.isPublic) {
            return <p>{"Public"}</p>;
        } else if (game.parameters.allowGuests) {
            return <p>{"Guests allowed"}</p>;
        } else {
            return "";
        }
    }

    private renderStartButton(game: Game){
        if (game === null) {
            return "";
        }

        //Only creator can start
        if (this.props.user.id !== game.createdByUserId) {
            return "";
        }

        //Can only start with at least 2 players
        if (game.players.length < 2){
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
        if (this.state.gameId === null) {
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
                {this.renderLobbyDetails(this.state.game)}
                <GamePlayersTable
                    user={this.props.user}
                    api={this.props.api}
                    game={this.state.game}
                    updateGame={newGame => this.playerTableUpdateGame(newGame)}
                />
                <br/>
                {this.renderStartButton(this.state.game)}
            </div>
        );
    }
}