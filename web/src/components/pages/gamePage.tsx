import * as React from 'react';
import ApiClient from '../../api/client';
import { User, Game } from '../../api/model';
import LinkButton from '../controls/linkButton';
import PageTitle from '../pageTitle';
import GameBoard from '../gameBoard';
import BoardView from '../../boardRendering/boardView';
import BoardViewFactory from '../../boardRendering/boardViewFactory';
import Routes from '../../routes';
import ThemeService from '../../themes/themeService';

export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number,
    theme : ThemeService
}

export interface GamePageState {
    game : Game,
    boardView : BoardView
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    constructor(props : GamePageProps) {
        super(props);
        this.state = {
            game : null,
            boardView : null
        };
    }

    private getCellSize(regionCount : number) : number {
        return Math.floor(160 * Math.pow(Math.E, (-0.2 * regionCount)));
    }

    componentDidMount() {
        this.props.api
            .getGame(this.props.gameId)
            .then(game => {
                this.setState({game : game});
                return this.props.api
                    .getBoard(game.parameters.regionCount)
                    .then(board => {
                        const cellSize = this.getCellSize(board.regionCount);
                        const boardView = BoardViewFactory.createBoard(board, cellSize);
                        this.setState({boardView : boardView});
                    })
                    .catch(reason => {
                        alert("Get board failed because " + reason);
                    });
            })
            .catch(reason => {
                alert("Get game failed because " + reason);
            });
    }

    render() {
        return (
            <div>
                <PageTitle label={"Game"}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to={Routes.dashboard()}/>
                    <LinkButton label="Rules" to={Routes.rules()} newWindow={true}/>
                </div>
                <br/>
                <GameBoard
                    user={this.props.user}
                    api={this.props.api}
                    game={this.state.game}
                    boardView={this.state.boardView}
                    theme={this.props.theme}
                />
            </div>
        );
    }
}