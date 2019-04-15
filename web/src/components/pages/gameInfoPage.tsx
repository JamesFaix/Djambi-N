import * as React from 'react';
import GameInfoPlayersTable from '../tables/gameInfoPlayersTable';
import PageTitle from '../pageTitle';
import {
    CreatePlayerRequest,
    Game,
    GameStatus,
    User
    } from '../../api/model';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';
import Button, { ButtonKind } from '../controls/button';
import { IconKind } from '../icons/icon';
import { EnterButton, FindGamesPageButton, CreateGamePageButton, MyGamesPageButton, DashboardPageButton } from '../controls/navigationButtons';
import GameStatusIcon from '../icons/gameStatusIcon';

export interface GameInfoPageProps {
    user : User,
    gameId : number,
    game : Game,
    load : (game:Game) => Promise<void>
}

export interface GameInfoPageState {
    guestName : string,
    redirectUrl : string
}

export default class GameInfoPage extends React.Component<GameInfoPageProps, GameInfoPageState> {

    constructor(props : GameInfoPageProps) {
        super(props);
        this.state = {
            guestName : "",
            redirectUrl : null
        };
    }

    public componentDidMount() : void {
        K.api
            .getGame(this.props.gameId)
            .then(game => this.props.load(game))
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
        this.props.load(newGame)
        .then(_ => {
            if (newGame.status === GameStatus.Canceled) {
                this.setState({
                    redirectUrl: K.routes.home()
                });
            }
        })
    }

    private startOnClick() {
        K.api
            .startGame(this.props.game.id)
            .then(_ => {
                this.setState({ redirectUrl : K.routes.game(this.props.game.id) });
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
            <div className={K.classes.table}>
                <div className={K.classes.centerAligned}>
                    {this.renderGameDescription(game)}
                    {this.renderGameOptions(game)}
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
        const regions = game.parameters.regionCount + " regions";

        if (game.parameters.isPublic && game.parameters.allowGuests) {
            return <p>{"Public, guests allowed, " + regions}</p>;
        } else if (game.parameters.isPublic) {
            return <p>{"Public, " + regions}</p>;
        } else if (game.parameters.allowGuests) {
            return <p>{"Guests allowed, " + regions}</p>;
        } else {
            return regions;
        }
    }

    private renderGameStatus(game: Game){
        if (game === null) {
            return "";
        }

        return (
            <div
                className={K.classes.centerAligned}
                style={{display:"inline-flex"}}
            >
                <div style={{padding:"6px 10px 6px 10px"}}>
                    <GameStatusIcon status={game.status}/>
                </div>
                {this.renderActionButton(game)}
            </div>
        );
    }

    private renderActionButton(game : Game) {
        if (game.status === GameStatus.InProgress ||
            game.status === GameStatus.Over) {

            return (
                <EnterButton
                    to={K.routes.game(this.props.game.id)}
                    hint="View game"
                />
            );
        }

        if (game.status === GameStatus.Pending &&
            game.players.length > 1 &&
            this.props.user.id === game.createdBy.userId) {

            return (
                <Button
                    kind={ButtonKind.Action}
                    icon={IconKind.InProgress}
                    onClick={() => this.startOnClick()}
                    hint="Start game"
                />
            );
        }

        return undefined;
    }

    public render() : JSX.Element {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to={K.routes.home()}/>;
        }

        if (this.state.redirectUrl !== null) {
            return <Redirect to={this.state.redirectUrl}/>
        }

        const title = "Game " + this.props.gameId;

        return (
            <div>
                <PageTitle label={title}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <DashboardPageButton/>
                    <MyGamesPageButton/>
                    <CreateGamePageButton/>
                    <FindGamesPageButton/>
                </div>
                {this.renderLobbyDetails(this.props.game)}
                <br/>
                <GameInfoPlayersTable
                    user={this.props.user}
                    game={this.props.game}
                    addPlayer={(gameId, request) => this.addPlayer(gameId, request)}
                    removePlayer={(gameId, playerId) => this.removePlayer(gameId, playerId)}
                />
            </div>
        );
    }
}