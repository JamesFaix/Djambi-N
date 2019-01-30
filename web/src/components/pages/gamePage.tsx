import * as React from 'react';
import ApiClient from '../../api/client';
import { User, Game, Board } from '../../api/model';
import LinkButton from '../controls/linkButton';
import PageTitle from '../pageTitle';
import Routes from '../../routes';
import ThemeService from '../../themes/themeService';
import { BoardView, CellView, CellState } from '../../boardRendering/model';
import CanvasBoard from '../board/canvasBoard';
import BoardViewService from '../../boardRendering/boardViewService';

export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number,
    theme : ThemeService
}

export interface GamePageState {
    game : Game,
    boardView : BoardView,
    board : Board
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    constructor(props : GamePageProps) {
        super(props);
        this.state = {
            game : null,
            boardView : null,
            board : null
        };
    }

    private getCellSize(regionCount : number) : number {
        return Math.floor(160 * Math.pow(Math.E, (-0.2 * regionCount)));
    }

    private async getAndCacheBoard(regionCount : number) : Promise<Board> {
        if (this.state.board) {
            return this.state.board;
        }

        return await this.props.api
            .getBoard(regionCount)
            .then(board => {
                this.setState({board : board});
                return board;
            })
            .catch(reason => {
                alert("Get board failed because " + reason);
                return null;
            });
    }

    private async updateState(game : Game) : Promise<void> {
        return await this.getAndCacheBoard(game.parameters.regionCount)
            .then(board => {
                const cellSize = this.getCellSize(game.parameters.regionCount);
                let boardView = BoardViewService.createBoard(board, cellSize);
                boardView = BoardViewService.update(boardView, game);
                this.setState({
                    boardView : boardView,
                    game : game
                });
            });
    }

    private onCellClick(cell : CellView) : void {
        console.log("Cell click ID:" + cell.id);
        if (cell.state === CellState.Selectable) {
            this.props.api
                .selectCell(this.props.gameId, cell.id)
                .then(response => this.updateState(response.game));
        }
    }

    componentDidMount() {
        this.props.api
            .getGame(this.props.gameId)
            .then(game => this.updateState(game));
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
                {
                    this.state.boardView
                    ?
                    <CanvasBoard
                        board={this.state.boardView}
                        theme={this.props.theme}
                        selectCell={(cellId) => this.onCellClick(cellId)}
                    />
                    : ""
                }
            </div>
        );
    }
}