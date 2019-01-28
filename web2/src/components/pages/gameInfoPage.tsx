import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User, Game, GameStatus } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';
import GamePlayersTable from '../gamePlayersTable';

export interface GameInfoPageProps {
    user : User,
    api : ApiClient,
    gameId : number
}

export interface GameInfoPageState {
    game : Game,
    guestName : string,
    gameId : number,
    redirectToGame : boolean
}

export default class GameInfoPage extends React.Component<GameInfoPageProps, GameInfoPageState> {

    constructor(props : GameInfoPageProps) {
        super(props);
        this.state = {
            game : null,
            guestName : "",
            gameId : props.gameId,
            redirectToGame : false
        };
    }

    componentDidMount() {
        this.props.api
            .getGame(this.props.gameId)
            .then(game => {
                this.setState({game : game});
            })
            .catch(reason => {
                alert("Get game failed because " + reason);
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
            .then(_ => {
                this.setState({ redirectToGame : true });
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

    private renderActionButton(game: Game){
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

        switch (game.status) {
            case GameStatus.Pending:
                return (
                    <div className="centeredContainer">
                        Pending
                        <ActionButton
                            label="Start"
                            onClick={() => this.startOnClick()}
                        />
                    </div>
                );

            case GameStatus.Started:
                return (
                    <div className="centeredContainer">
                        Started
                        <LinkButton
                            label="Enter"
                            to={"/games/" + this.props.gameId}
                        />
                    </div>
                );

            case GameStatus.Finished:
                return (
                    <div className="centeredContainer">
                        Finished
                    </div>
                );
            case GameStatus.Aborted:
            case GameStatus.AbortedWhilePending:
                return (
                    <div className="centeredContainer">
                        Aborted
                    </div>
                );
        }
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

        if (this.state.redirectToGame) {
            return <Redirect to={'/games/' + this.state.gameId}/>;
        }

        return (
            <div>
                <PageTitle label={"Game Info"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="My Games" to="/games/my"/>
                    <LinkButton label="Create Game" to="/games/create"/>
                    <LinkButton label="Find Game" to="/games/find"/>
                </div>
                {this.renderLobbyDetails(this.state.game)}
                <GamePlayersTable
                    user={this.props.user}
                    api={this.props.api}
                    game={this.state.game}
                    updateGame={newGame => this.playerTableUpdateGame(newGame)}
                />
                <br/>
                {this.renderActionButton(this.state.game)}
            </div>
        );
    }
}