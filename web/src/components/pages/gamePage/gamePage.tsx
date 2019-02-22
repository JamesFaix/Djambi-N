import * as React from 'react';
import { User, Game, Event, EventsQuery, ResultsDirection } from '../../../api/model';
import LinkButton from '../../controls/linkButton';
import PageTitle from '../../pageTitle';
import Routes from '../../../routes';
import { BoardView, CellView, CellState, Point } from '../../../boardRendering/model';
import CurrentTurnPanel from './currentTurnPanel';
import TurnCyclePanel from './turnCyclePanel';
import PlayersPanel from './playersPanel/playersPanel';
import HistoryPanel from './historyPanel/historyPanel';
import { Classes, Styles } from '../../../styles';
import BoardPanel from './boardPanel/boardPanel';
import Geometry from '../../../boardRendering/geometry';
import Kernel from '../../../kernel';

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

    constructor(props : GamePageProps) {
        super(props);
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
        const boardView = await Kernel.boardViews.getBoardView(game);
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
        const events = await Kernel.api.getEvents(gameId, eventQuery);
        this.setState({ events: events });
    }

    private selectCell(cell : CellView) : void {
        if (cell.state === CellState.Selectable) {
            Kernel.api
                .selectCell(this.props.gameId, cell.id)
                .then(response => this.updateGame(response.game));
        }
    }

    private commitTurn(gameId : number) : void {
        Kernel.api
            .commitTurn(gameId)
            .then(response => this.updateGame(response.game))
            .then(_ => this.updateEvents(gameId));
    }

    private resetTurn(gameId : number) : void {
        Kernel.api
            .resetTurn(gameId)
            .then(response => this.updateGame(response.game));
    }

    componentDidMount() {
        Kernel.api
            .getGame(this.props.gameId)
            .then(game => this.updateGame(game))
            .then(_ => this.updateEvents(this.props.gameId));
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
                {this.renderPanels()}
            </div>
        );
    }

    private renderPanels() {
        //The game is fetched from the API on page load. There's not much to render before that.
        if (this.state.game === null || this.state.boardView === null) {
            return "";
        }

        const containerStyle = Styles.combine([
            Styles.noMargin,
            Styles.width(this.contentSize.x + "px"),
            Styles.height(this.contentSize.y + "px")
        ]);

        const textStyle = Styles.lineHeight("8px");

        const boardPanelSize = {
            x: this.contentSize.x * 0.7,
            y: this.contentSize.y
        };

        return (
            <div className={Classes.flex} style={containerStyle}>
                <div style={Styles.width("70%")}>
                    <BoardPanel
                        game={this.state.game}
                        boardView={this.state.boardView}
                        selectCell={cell => this.selectCell(cell)}
                        size={boardPanelSize}
                        boardStrokeWidth={10}
                        boardMargin={5}
                    />
                </div>
                <div style={Styles.width("30%")}>
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
                    />
                </div>
            </div>
        );
    }
}