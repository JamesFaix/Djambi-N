import * as React from 'react';
import BoardPanel from '../panels/boardPanel';
import Geometry from '../../boardRendering/geometry';
import HistoryPanel from '../panels/historyPanel';
import PlayersPanel from '../panels/playersPanel';
import TurnCyclePanel from '../panels/turnCyclePanel';
import {
    BoardView,
    CellState,
    CellView,
    Point
    } from '../../boardRendering/model';
import {
    Event,
    EventsQuery,
    Game,
    ResultsDirection,
    User,
    PlayerStatus,
    Player,
    GameStatus,
    StateAndEventResponse
    } from '../../api/model';
import { Kernel as K } from '../../kernel';
import ActionPanel from '../panels/actionPanel';
import PlayerActionsService from '../../playerActionsService';
import { Redirect } from 'react-router';
import StatusChangeModal from '../modals/statusChangeModal';
import { RulesPageButton, DashboardPageButton } from '../controls/navigationButtons';
import GameOverModal from '../modals/gameOverModal';

export interface GamePageProps {
    user : User,
    gameId : number,
    game : Game,
    boardView : BoardView,
    history : Event[],
    update : (response:StateAndEventResponse) => Promise<void>,
    load : (game:Game, history:Event[]) => Promise<void>
}

export interface GamePageState {
    redirectUrl : string,
    statusChangeModalStatus : PlayerStatus,
    statusChangeModalPlayer : Player,
    showGameOverModal : boolean
}

export default class GamePage extends React.Component<GamePageProps, GamePageState> {
    private readonly contentSize : Point;

    constructor(props : GamePageProps) {
        super(props);

        this.state = {
            redirectUrl: null,
            statusChangeModalStatus: null,
            statusChangeModalPlayer: null,
            showGameOverModal: false
        };

        const windowSize = {
            x: window.screen.width,
            y: window.screen.height
        };
        this.contentSize = Geometry.Point.multiplyScalar(windowSize, 0.6);
    }

    private async update(response : StateAndEventResponse) : Promise<void> {
        await this.props.update(response)
            .then(_ => {
                this.setState({
                    showGameOverModal : response.game.status === GameStatus.Over
                });
            });
    }

    private async load() : Promise<void> {
        const eventQuery : EventsQuery = {
            maxResults: null,
            direction: ResultsDirection.Descending,
            thresholdEventId: null,
            thresholdTime: null
        }
        const game = await K.api.getGame(this.props.gameId);
        const history = await K.api.getEvents(this.props.gameId, eventQuery);
        await this.props.load(game, history);
    }

    private selectCell(cell : CellView) : void {
        if (cell.state === CellState.Selectable) {
            K.api
                .selectCell(this.props.gameId, cell.id)
                .then(response => this.update(response));
        }
    }

    //--- PlayerActionsController ---

    commitTurn() : void {
        K.api
            .commitTurn(this.props.gameId)
            .then(response => this.update(response));
    }

    resetTurn() : void {
        K.api
            .resetTurn(this.props.gameId)
            .then(response => this.update(response));
    }

    openStatusModal(status : PlayerStatus) : void {
        this.setState({statusChangeModalStatus: status});
    }

    navigateToSnapshotsPage() : void {
        this.setState({redirectUrl: K.routes.snapshots(this.props.gameId)});
    }

    public componentDidMount() : void {
        this.load();
    }

    //--- ---

    private onStatusChangeModalSelectPlayer(player : Player) {
        this.setState({statusChangeModalPlayer: player});
    }

    private onStatusChangeModalCancel() {
        this.setState({
            statusChangeModalStatus: null,
            statusChangeModalPlayer: null
        });
    }

    private onStatusChangeModalOk() {
        K.api
            .updatePlayerStatus(this.props.gameId, this.state.statusChangeModalPlayer.id, this.state.statusChangeModalStatus)
            .then(response => this.update(response))
            .then(_ => this.onStatusChangeModalCancel());
    }

    private onGameOverModalOk() {
        this.setState({showGameOverModal: false});
    }

    public render() : JSX.Element {
        if (this.state.redirectUrl !== null) {
            return <Redirect to={this.state.redirectUrl}/>;
        }

        return (
            <div>
                <br/>
                <br/>
                <br/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <DashboardPageButton/>
                    <RulesPageButton/>
                </div>
                <br/>
                {this.renderPanels()}
                {this.renderStatusChangeModal()}
                {this.renderGameOverModal()}
            </div>
        );
    }

    private renderPanels() {
        //The game is fetched from the API on page load. There's not much to render before that.
        if (this.props.game === null || this.props.boardView === null) {
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

        const playerActionsService = new PlayerActionsService(
            this.props.user,
            this.props.game,
            this
        );

        return (
            <div className={K.classes.flex} style={containerStyle}>
                <div style={K.styles.width("70%")}>
                    <BoardPanel
                        game={this.props.game}
                        boardView={this.props.boardView}
                        selectCell={cell => this.selectCell(cell)}
                        size={boardPanelSize}
                        boardStrokeWidth={5}
                        boardMargin={5}
                    />
                </div>
                <div style={{
                    width: "30%",
                    display: "flex",
                    flexDirection: "column"
                }}>
                    <TurnCyclePanel
                        game={this.props.game}
                        iconSize={"20px"}
                        width={"100%"}
                    />
                    <PlayersPanel
                        game={this.props.game}
                        width={"100%"}
                    />
                    <ActionPanel
                        game={this.props.game}
                        user={this.props.user}
                        width={"100%"}
                        playerActionsService={playerActionsService}
                    />
                    <HistoryPanel
                        game={this.props.game}
                        user={this.props.user}
                        events={this.props.history}
                        width={"100%"}
                        textStyle={textStyle}
                        getBoard={n => K.boards.getBoardIfCached(n)}
                    />
                </div>
            </div>
        );
    }

    private renderStatusChangeModal() {
        const status = this.state.statusChangeModalStatus;
        if (status === null) {
            return undefined;
        }

        const service = new PlayerActionsService(this.props.user, this.props.game, this);
        const players = service.controllablePlayersThatCanChangeToStatus(status);

        return (
            <StatusChangeModal
                targetStatus={status}
                playerOptions={players}
                setPlayer={player => this.onStatusChangeModalSelectPlayer(player)}
                onOk={() => this.onStatusChangeModalOk()}
                onCancel={() => this.onStatusChangeModalCancel()}
            />
        );
    }

    private renderGameOverModal() {
        if (!this.state.showGameOverModal) {
            return undefined;
        }

        return (
            <GameOverModal
                game={this.props.game}
                closeWindow={() => this.onGameOverModalOk()}
            />
        );
    }
}