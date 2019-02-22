import * as React from 'react';
import { User, Game, Event, EventsQuery, ResultsDirection } from '../../../api/model';
import LinkButton from '../../controls/linkButton';
import { BoardView, CellView, CellState, Point } from '../../../boardRendering/model';
import CurrentTurnPanel from './currentTurnPanel';
import TurnCyclePanel from './turnCyclePanel';
import PlayersPanel from './playersPanel/playersPanel';
import HistoryPanel from './historyPanel/historyPanel';
import BoardPanel from './boardPanel/boardPanel';
import Geometry from '../../../boardRendering/geometry';
import {Kernel as K} from '../../../kernel';
import BoardViewService from '../../../boardRendering/boardViewService';

export interface GamePageProps {
    user : User,
    gameId : number
}

export interface GamePageState {
    game : Game,
    boardView : BoardView,
    events : Event[]
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    private readonly contentSize : Point;
    private readonly boardViewService : BoardViewService;

    constructor(props : GamePageProps) {
        super(props);

        this.boardViewService = new BoardViewService(K.boards);

        this.state = {
            game : null,
            boardView : null,
            events: []
        };

        const windowSize = {
            x: window.screen.width,
            y: window.screen.height
        };
        this.contentSize = Geometry.Point.multiplyScalar(windowSize, 0.6);
    }

    private async updateGame(game : Game) : Promise<void> {
        const boardView = await this.boardViewService.getBoardView(game);
        this.setState({
            boardView : boardView,
            game : game
        });
    }

    private async updateEvents(gameId: number) : Promise<void> {
        const eventQuery : EventsQuery = {
            maxResults: null,
            direction: ResultsDirection.Descending,
            thresholdEventId: null,
            thresholdTime: null
        }
        const events = await K.api.getEvents(gameId, eventQuery);
        this.setState({ events: events });
    }

    private selectCell(cell : CellView) : void {
        if (cell.state === CellState.Selectable) {
            K.api
                .selectCell(this.props.gameId, cell.id)
                .then(response => this.updateGame(response.game));
        }
    }

    private commitTurn(gameId : number) : void {
        K.api
            .commitTurn(gameId)
            .then(response => this.updateGame(response.game))
            .then(_ => this.updateEvents(gameId));
    }

    private resetTurn(gameId : number) : void {
        K.api
            .resetTurn(gameId)
            .then(response => this.updateGame(response.game));
    }

    componentDidMount() {
        K.api
            .getGame(this.props.gameId)
            .then(game => this.updateGame(game))
            .then(_ => this.updateEvents(this.props.gameId));
    }

    render() {
        return (
            <div>
                <br/>
                <br/>
                <br/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <LinkButton label="Home" to={K.routes.dashboard()}/>
                    <LinkButton label="Rules" to={K.routes.rules()} newWindow={true}/>
                </div>
                <br/>
                {this.renderPanels()}
            </div>
        );
    }

    private renderPanels() {
        //The game is fetched from the API on page load. There's not much to render before that.
        if (this.state.game === null || this.state.boardView === null) {
            return "";
        }

        const containerStyle = K.styles.combine([
            K.styles.noMargin,
            K.styles.width(this.contentSize.x + "px"),
            K.styles.height(this.contentSize.y + "px")
        ]);

        const textStyle = K.styles.lineHeight("8px");

        const boardPanelSize = {
            x: this.contentSize.x * 0.7,
            y: this.contentSize.y
        };

        return (
            <div className={K.classes.flex} style={containerStyle}>
                <div style={K.styles.width("70%")}>
                    <BoardPanel
                        game={this.state.game}
                        boardView={this.state.boardView}
                        selectCell={cell => this.selectCell(cell)}
                        size={boardPanelSize}
                        boardStrokeWidth={10}
                        boardMargin={5}
                    />
                </div>
                <div style={K.styles.width("30%")}>
                    <TurnCyclePanel
                        game={this.state.game}
                        iconSize={"20px"}
                        height={"50px"}
                        width={"100%"}
                    />
                    <PlayersPanel
                        game={this.state.game}
                        height={"100px"}
                        width={"100%"}
                    />
                    <CurrentTurnPanel
                        game={this.state.game}
                        user={this.props.user}
                        commitTurn={gameId => this.commitTurn(gameId)}
                        resetTurn={gameId => this.resetTurn(gameId)}
                        height={"150px"}
                        width={"100%"}
                        textStyle={textStyle}
                    />
                    <HistoryPanel
                        game={this.state.game}
                        events={this.state.events}
                        height={"350px"}
                        width={"100%"}
                        textStyle={textStyle}
                        getBoard={n => K.boards.getBoardIfCached(n)}
                    />
                </div>
            </div>
        );
    }
}