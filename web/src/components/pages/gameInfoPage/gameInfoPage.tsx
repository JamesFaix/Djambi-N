import * as React from 'react';
import PageTitle from '../../pageTitle';
import { User, Game, GameStatus, CreatePlayerRequest } from '../../../api/model';
import { Redirect } from 'react-router';
import LinkButton from '../../controls/linkButton';
import ActionButton from '../../controls/actionButton';
import GameInfoPlayersTable from './gameInfoPlayersTable';
import Routes from '../../../routes';
import { Classes } from '../../../styles';
import { Kernel as K } from '../../../kernel';

export interface GameInfoPageProps {
    user : User,
    gameId : number
}

export interface GameInfoPageState {
    game : Game,
    guestName : string,
    redirectUrl : string
}

export default class GameInfoPage extends React.Component<GameInfoPageProps, GameInfoPageState> {

    constructor(props : GameInfoPageProps) {
        super(props);
        this.state = {
            game : null,
            guestName : "",
            redirectUrl : null
        };
    }

    componentDidMount() {
        K.api
            .getGame(this.props.gameId)
            .then(game => {
                this.setState({game : game});
            })
            .catch(reason => {
                alert("Get game failed because " + reason);
            });
    }

//---Event handlers---

    private addPlayer(gameId : number, request : CreatePlayerRequest) : void {
        K.api.addPlayer(gameId, request)
            .then(response => this.updateGame(response.game));
    }

    private removePlayer(gameId : number, playerId : number) : void {
        K.api.removePlayer(gameId, playerId)
            .then(response => this.updateGame(response.game));
    }

    private updateGame(newGame : Game) {
        if (newGame.status === GameStatus.Aborted
            || newGame.status === GameStatus.AbortedWhilePending) {
            this.setState({
                game : newGame,
                redirectUrl: Routes.home()
            });
        } else {
            this.setState({game : newGame});
        }
    }

    private startOnClick() {
        K.api
            .startGame(this.state.game.id)
            .then(_ => {
                this.setState({ redirectUrl : Routes.game(this.state.game.id) });
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
            <div className={Classes.table}>
                <div className={Classes.centerAligned}>
                    {this.renderGameDescription(game)}
                    {this.renderGameOptions(game)}
                    <p>{game.parameters.regionCount + " regions"}</p>
                    {this.renderGameStatus(game)}
                </div>
            </div>
        );
    }

    private renderGameDescription(game : Game) {
        if (game.parameters.description === null) {
            return "";
        } else {
            return <p>{game.parameters.description}</p>;
        }
    }

    private renderGameOptions(game : Game) {
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

    private renderGameStatus(game: Game){
        if (game === null) {
            return "";
        }

        switch (game.status) {
            case GameStatus.Pending:
                return (
                    //Only creator can start game, and only with > 1 players
                    <div className={Classes.centerAligned}>
                        Pending
                        { this.props.user.id === game.createdByUserId
                            && game.players.length >= 2
                            ? <ActionButton
                                label="Start"
                                onClick={() => this.startOnClick()}
                            />
                            : ""
                        }
                    </div>
                );

            case GameStatus.Started:
                return (
                    <div className={Classes.centerAligned}>
                        Started
                        <LinkButton label="Enter" to={Routes.game(this.state.game.id)} />
                    </div>
                );

            case GameStatus.Finished:
                return (
                    <div className={Classes.centerAligned}>
                        Finished
                    </div>
                );
            case GameStatus.Aborted:
            case GameStatus.AbortedWhilePending:
                return (
                    <div className={Classes.centerAligned}>
                        Aborted
                    </div>
                );
        }
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to={Routes.home()}/>;
        }

        if (this.state.redirectUrl !== null) {
            return <Redirect to={this.state.redirectUrl}/>
        }

        return (
            <div>
                <PageTitle label={"Game Info"}/>
                <br/>
                <div className={Classes.centerAligned}>
                    <LinkButton label="Home" to={Routes.dashboard()}/>
                    <LinkButton label="My Games" to={Routes.myGames()}/>
                    <LinkButton label="Create Game" to={Routes.createGame()}/>
                    <LinkButton label="Find Game" to={Routes.findGame()}/>
                </div>
                {this.renderLobbyDetails(this.state.game)}
                <br/>
                <GameInfoPlayersTable
                    user={this.props.user}
                    game={this.state.game}
                    addPlayer={(gameId, request) => this.addPlayer(gameId, request)}
                    removePlayer={(gameId, playerId) => this.removePlayer(gameId, playerId)}
                />
            </div>
        );
    }
}