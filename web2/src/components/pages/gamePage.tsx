import * as React from 'react';
import ApiClient from '../../api/client';
import { User, Game } from '../../api/model';
import LinkButton from '../linkButton';
import PageTitle from '../pageTitle';
import GameBoard from '../gameBoard';
import BoardView from '../../display/boardView';
import BoardViewFactory from '../../display/boardViewFactory';

export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number,
    rulesUrl : string
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

    private cellSize = 10;

    componentDidMount() {
        this.props.api
            .getGame(this.props.gameId)
            .then(game => {
                this.setState({game : game});
                return this.props.api
                    .getBoard(game.parameters.regionCount)
                    .then(board => {
                        let boardView = BoardViewFactory.createBoard(board, this.cellSize);
                        this.setState({boardView : boardView});
                        console.log(boardView);
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
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="Rules" to={this.props.rulesUrl} newWindow={true}/>
                </div>
                <br/>
                <GameBoard
                    user={this.props.user}
                    api={this.props.api}
                    game={this.state.game}
                    boardView={this.state.boardView}
                />
            </div>
        );
    }
}