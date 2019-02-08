import * as React from 'react';
import ApiClient from '../../../api/client';
import { User, Game, Event, EventsQuery, ResultsDirection } from '../../../api/model';
import LinkButton from '../../controls/linkButton';
import PageTitle from '../../pageTitle';
import Routes from '../../../routes';
import ThemeService from '../../../themes/themeService';
import { BoardView, CellView, CellState } from '../../../boardRendering/model';
import BoardViewService from '../../../boardRendering/boardViewService';
import CurrentTurnPanel from './currentTurnPanel';
import TurnCyclePanel from './turnCyclePanel';
import PlayersPanel from './playersPanel/playersPanel';
import HistoryPanel from './historyPanel/historyPanel';
import { Classes, Styles } from '../../../styles';
import BoardPanel from './boardPanel/boardPanel';
import BoardService from '../../../boardService';

export interface GamePageProps {
    user : User,
    api : ApiClient,
    gameId : number,
    theme : ThemeService,
    boardService : BoardService
}

export interface GamePageState {
    game : Game,
    boardView : BoardView,
    events : Event[]
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    private readonly contentWidth = Math.round(window.screen.width * 0.6) + "px";
    private readonly contentHeight = Math.round(window.screen.height * 0.6) + "px";
    private readonly scale = 100;

    constructor(props : GamePageProps) {
        super(props);
        this.state = {
            game : null,
            boardView : null,
            events: []
        };
    }

    private getCellSize(regionCount : number) : number {
        //Through trial an error, I found that this formula keeps boards of varying regionCount about the same absolute size
        const baseValue = Math.pow(Math.E, (-0.2 * regionCount));
        return Math.floor(this.scale * baseValue);
    }

    private async updateState(game : Game) : Promise<void> {
        return await this.props.boardService.getBoard(game.parameters.regionCount)
            .then(board => this.getEvents(game.id)
                .then(events => {
                    const cellSize = this.getCellSize(game.parameters.regionCount);
                    const boardView = BoardViewService.getBoard(board, cellSize, game);
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
            Styles.width(this.contentWidth),
            Styles.height(this.contentHeight)
        ]);

        const textStyle = Styles.lineHeight("8px");

        return (
            <div className={Classes.flex} style={containerStyle}>
                <BoardPanel
                    game={this.state.game}
                    theme={this.props.theme}
                    boardView={this.state.boardView}
                    selectCell={cell => this.selectCell(cell)}
                    height={"100%"}
                    width={"70%"}
                />
                <div style={Styles.width("30%")}>
                    <TurnCyclePanel
                        game={this.state.game}
                        theme={this.props.theme}
                        iconSize={"20px"}
                        height={"50px"}
                        width={"100%"}
                    />
                    <PlayersPanel
                        game={this.state.game}
                        theme={this.props.theme}
                        height={"100px"}
                        width={"100%"}
                    />
                    <CurrentTurnPanel
                        game={this.state.game}
                        theme={this.props.theme}
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
                        theme={this.props.theme}
                        height={"350px"}
                        width={"100%"}
                        textStyle={textStyle}
                    />
                </div>
            </div>
        );
    }
}