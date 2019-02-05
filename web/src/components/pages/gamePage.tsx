import * as React from 'react';
import ApiClient from '../../api/client';
import { User, Game, Board, Event, EventsQuery, ResultsDirection } from '../../api/model';
import LinkButton from '../controls/linkButton';
import PageTitle from '../pageTitle';
import Routes from '../../routes';
import ThemeService from '../../themes/themeService';
import { BoardView, CellView, CellState } from '../../boardRendering/model';
import CanvasBoard from '../board/canvasBoard';
import BoardViewService from '../../boardRendering/boardViewService';
import BoardGeometry from '../../boardRendering/boardGeometry';
import CurrentTurnPanel from '../gamePanels/currentTurnPanel';
import TurnCyclePanel from '../gamePanels/turnCyclePanel';
import PlayersPanel from '../gamePanels/playersPanel';
import HistoryPanel from '../gamePanels/historyPanel';
import { Classes, Styles } from '../../styles';

export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number,
    theme : ThemeService
}

export interface GamePageState {
    game : Game,
    boardView : BoardView,
    board : Board,
    events : Event[]
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    private readonly scale = 100;

    constructor(props : GamePageProps) {
        super(props);
        this.state = {
            game : null,
            boardView : null,
            board : null,
            events: []
        };
    }

    private getCellSize(regionCount : number) : number {
        //Through trial an error, I found that this formula keeps boards of varying regionCount about the same absolute size
        const baseValue = Math.pow(Math.E, (-0.2 * regionCount));
        return Math.floor(this.scale * baseValue);
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
            .then(board => this.getEvents(game.id)
                .then(events => {
                    const cellSize = this.getCellSize(game.parameters.regionCount);
                    let boardView = BoardViewService.createBoard(board, cellSize);
                    boardView = BoardViewService.update(boardView, game);
                    this.setState({
                        boardView : boardView,
                        game : game,
                        events: events
                    });
                })
            );
    }

    private selectCell(cell : CellView) : void {
        if (cell.state === CellState.Selectable) {
            this.props.api
                .selectCell(this.props.gameId, cell.id)
                .then(response => this.updateState(response.game));
        }
    }

    private commitTurn(gameId : number) : void {
        this.props.api
            .commitTurn(gameId)
            .then(response => this.updateState(response.game));
    }

    private resetTurn(gameId : number) : void {
        this.props.api
            .resetTurn(gameId)
            .then(response => this.updateState(response.game));
    }

    private getEvents(gameId : number) : Promise<Event[]> {
        const eventQuery : EventsQuery = {
            maxResults: null,
            direction: ResultsDirection.Descending,
            thresholdEventId: null,
            thresholdTime: null
        }

        return this.props.api.getEvents(gameId, eventQuery);
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
                <div className={Classes.centerAligned}>
                    <LinkButton label="Home" to={Routes.dashboard()}/>
                    <LinkButton label="Rules" to={Routes.rules()} newWindow={true}/>
                </div>
                <br/>
                {this.renderBoard()}
            </div>
        );
    }

    private renderBoard() {
        if (this.state.boardView === null) {
            return "";
        }

        const canvasSize = BoardGeometry.boardDiameter(this.state.boardView);

        return (
            <div style={Styles.gamePageContainer(canvasSize)}>
                <div
                    className={Classes.thinBorder}
                    style={Styles.gamePageCanvas(canvasSize)}
                >
                    <CanvasBoard
                        board={this.state.boardView}
                        theme={this.props.theme}
                        selectCell={(cellId) => this.selectCell(cellId)}
                    />
                </div>
                <div className={Classes.flex}>
                    <CurrentTurnPanel
                        game={this.state.game}
                        theme={this.props.theme}
                        user={this.props.user}
                        commitTurn={gameId => this.commitTurn(gameId)}
                        resetTurn={gameId => this.resetTurn(gameId)}
                    />
                    <div style={Styles.width(60)}>
                        <TurnCyclePanel
                            game={this.state.game}
                            theme={this.props.theme}
                        />
                        <PlayersPanel
                            game={this.state.game}
                            theme={this.props.theme}
                        />
                    </div>
                </div>
                <HistoryPanel
                    game={this.state.game}
                    events={this.state.events}
                    theme={this.props.theme}
                />
            </div>
        );
    }
}