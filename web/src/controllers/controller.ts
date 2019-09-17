import { AppStore, CustomAction, State } from "../store/root";
import { History } from "history";
import { DebugSettings, defaultDebugSettings } from "../debug";
import LocalStorageService from "../utilities/localStorageService";
import * as StoreSettings from '../store/settings';
import * as StoreGamesQuery from '../store/search';
import * as StoreActiveGame from '../store/activeGame';
import * as StoreSession from '../store/session';
import * as StoreBoards from '../store/boards';
import * as StoreCreateGameForm from '../store/createGameForm';
import * as StoreDisplay from '../store/display';
import * as StoreNotifications from '../store/notifications';
import * as Api from '../api/client';
import {
    GamesQuery,
    CreateSnapshotRequest,
    LoginRequest,
    CreateUserRequest,
    User,
    EventsQuery,
    GameParameters,
    CreatePlayerRequest,
    StateAndEventResponse,
    GameStatus,
    Game,
    ResultsDirection,
    Event,
    Board,
    PlayerStatus,
    PieceKind
} from "../api/model";
import { SseClientManager } from "../utilities/serverSentEvents";
import ThemeService from "../themes/themeService";
import Routes from "../routes";
import * as ModelFactory from "../api/modelFactory";
import { Theme } from "../themes/model";
import ThemeFactory from "../themes/themeFactory";
import { isNumber } from "util";
import ApiUtil from "../api/util";
import Images, { PieceImageInfo } from "../utilities/images";
import Geometry from "../viewModel/board/geometry";
import { Point } from "../viewModel/board/model";
import { generateQuickGuid } from "../utilities/guids";
import Copy from "../utilities/copy";

//Encapsulates dispatching Redux actions and other side effects
export default class Controller {
    private static store : AppStore;
    private static history : History<any>;

    public static init(
        store : AppStore,
        history : History<any>
    ) : void {
        this.store = store;
        this.history = history;

        Controller.Settings.loadAndApply();
        Controller.Display.loadSavedTheme();
    }

    private static dispatch(action : CustomAction) {
        Controller.store.dispatch(action);
    }

    private static get state() : State {
        return Controller.store.getState();
    }

    public static navigateTo(route : string) : void {
        Controller.history.push(route);
        if (Controller.state.settings.debug.logRedux) {
            console.log(`Navigate to ${route}`);
        }
    }

    public static Settings = class {
        public static saveAndApply(settings : DebugSettings) : void {
            LocalStorageService.debugSettings = settings;
            Controller.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
        }

        public static loadAndApply() : void {
            let settings = LocalStorageService.debugSettings;
            if (!settings) { settings = defaultDebugSettings; }
            Controller.dispatch(StoreSettings.Actions.applyDebugSettings(settings));
        }
    }

    public static Search = class {
        public static async searchGames(query: GamesQuery) : Promise<void> {
            const games = await Api.getGames(query);
            Controller.dispatch(StoreGamesQuery.Actions.loadSearchResults(games));
        }

        public static async loadRecentGames(user : User) : Promise<void> {
            const query = ModelFactory.emptyGamesQuery();
            query.playerUserName = user.name;
            const games = await Api.getGames(query);
            Controller.dispatch(StoreGamesQuery.Actions.loadRecentGames(games));
        }
    }

    public static Snapshots = class {
        public static async get(gameId: number) : Promise<void> {
            const snapshots = await Api.getSnapshotsForGame(gameId);
            Controller.dispatch(StoreActiveGame.Actions.loadSnapshots(snapshots))
        }

        public static async save(gameId : number, request : CreateSnapshotRequest) : Promise<void> {
            const snapshot = await Api.createSnapshot(gameId, request);
            Controller.dispatch(StoreActiveGame.Actions.snapshotSaved(snapshot));
        }

        public static async load(gameId : number, snapshotId : number) : Promise<void> {
            await Api.loadSnapshot(gameId, snapshotId);
            return Controller.Game.loadGameFull(gameId);
        }

        public static async delete(gameId : number, snapshotId : number) : Promise<void>{
            await Api.loadSnapshot(gameId, snapshotId);
            Controller.dispatch(StoreActiveGame.Actions.snapshotDeleted(snapshotId));
        }
    }

    public static Session = class {
        public static async login(request : LoginRequest) : Promise<void> {
            const session = await Api.login(request);
            Controller.dispatch(StoreSession.Actions.login(session.user));
            Controller.navigateTo(Routes.dashboard);
            return Controller.Session.finishLoginSetup(session.user);
        }

        public static async logout() : Promise<void> {
            await Api.logout();
            Controller.Session.onUnauthorized();
        }

        public static onUnauthorized() : void {
            Controller.dispatch(StoreSession.Actions.logout());
            Controller.navigateTo(Routes.login);
            SseClientManager.disconnect();
        }

        public static async signup(request : CreateUserRequest) : Promise<void> {
            const user = await Api.createUser(request);
            Controller.dispatch(StoreSession.Actions.signup(user));
            const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
            return Controller.Session.login(loginRequest);
        }

        private static async getUser() : Promise<User> {
            let user = Controller.state.session.user;
            if (!user) {
                try {
                    user = await Api.getCurrentUser();
                    Controller.dispatch(StoreSession.Actions.restoreSession(user));
                    await Controller.Session.finishLoginSetup(user);
                }
                catch (ex) {
                    const [status, _] = ApiUtil.destructureError(ex);
                    if (status !== 401) {
                        throw ex;
                    }
                }
            }
            return user;
        }

        public static async redirectToDashboardIfLoggedIn() : Promise<void> {
            const user = await Controller.Session.getUser();
            if (user) {
                Controller.navigateTo(Routes.dashboard);
            }
        }

        public static async redirectToLoginIfNotLoggedIn() : Promise<void> {
            const user = await Controller.Session.getUser();
            if (!user) {
                Controller.navigateTo(Routes.login);
            }
        }

        public static async redirectToLoginOrDashboard() : Promise<void> {
            const user = await Controller.Session.getUser();
            const route = user ? Routes.dashboard : Routes.login;
            Controller.navigateTo(route);
        }

        private static finishLoginSetup(user : User) : Promise<void> {
            SseClientManager.connect();
            Controller.Display.loadSavedTheme();
            Controller.Settings.loadAndApply();
            return Controller.Search.loadRecentGames(user);
        }
    }

    public static Game = class {
        private static async loadGameInner(gameId : number) : Promise<Game> {
            Controller.dispatch(StoreActiveGame.Actions.clearGame());
            const game = await Api.getGame(gameId);
            Controller.dispatch(StoreActiveGame.Actions.loadGame(game));
            return game;
        }

        private static async loadHistoryInner(gameId : number) : Promise<Event[]> {
            const query : EventsQuery = {
                maxResults : null,
                direction : ResultsDirection.Descending,
                thresholdTime : null,
                thresholdEventId : null
            };

            const history = await Api.getEvents(gameId, query);
            Controller.dispatch(StoreActiveGame.Actions.loadGameHistory(history));
            return history;
        }

        public static async loadAllBoards() : Promise<void> {
            for (let i=3; i<=8; i++){
                await Controller.Game.loadBoardInner(i);
            }
        }

        private static async loadBoardInner(regionCount : number) : Promise<Board> {
            let board = Controller.state.boards.boards.get(regionCount);
            if (!board) {
                board = await Api.getBoard(regionCount);
                Controller.dispatch(StoreBoards.Actions.loadBoard(board));
            }
            return board;
        }

        public static async loadGame(gameId: number) : Promise<void> {
            await Controller.Game.loadGameInner(gameId);
        }

        public static async loadGameFull(gameId : number) : Promise<void> {
            const game = await Controller.Game.loadGameInner(gameId);
            await Controller.Game.loadHistoryInner(gameId);
            await Controller.Game.loadBoardInner(game.parameters.regionCount);
        }

        public static async loadGameIfNotLoaded(routeGameId : number) : Promise<Game> {
            let game = Controller.state.activeGame.game;
            if (!game || (game.id !== routeGameId && isNumber(routeGameId))) {
                game = await Controller.Game.loadGameInner(routeGameId);
            }
            return game;
        }

        public static async redirectToLobbyIfGameNotStatus(status : GameStatus) : Promise<void> {
            const game = Controller.state.activeGame.game;
            if (game.status !== status) {
                Controller.navigateTo(Routes.lobby(game.id));
            }
        }

        public static async createGame(formData : GameParameters) : Promise<void> {
            const game = await Api.createGame(formData);
            Controller.dispatch(StoreActiveGame.Actions.createGame(game));
            Controller.navigateTo(Routes.lobby(game.id));
        }

        public static async addPlayer(gameId: number, request : CreatePlayerRequest) : Promise<void> {
            const resp = await Api.addPlayer(gameId, request);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async removePlayer(gameId: number, playerId: number) : Promise<void> {
            const resp = await Api.removePlayer(gameId, playerId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async startGame(gameId: number) : Promise<void> {
            const resp = await Api.startGame(gameId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
            Controller.navigateTo(Routes.play(gameId));
        }

        public static async selectCell(gameId: number, cellId : number) : Promise<void> {
            const resp = await Api.selectCell(gameId, cellId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async endTurn(gameId: number) : Promise<void> {
            const resp = await Api.commitTurn(gameId);
            Controller.Game.updateInProgressGame(resp);
        }

        public static async resetTurn(gameId: number) : Promise<void> {
            const resp = await Api.resetTurn(gameId);
            Controller.dispatch(StoreActiveGame.Actions.updateGame(resp));
        }

        public static async changePlayerStatus(gameId: number, playerId: number, status: PlayerStatus) : Promise<void> {
            const resp = await Api.updatePlayerStatus(gameId, playerId, status);
            Controller.Game.updateInProgressGame(resp);
        }

        public static updateInProgressGame(response : StateAndEventResponse) : void {
            Controller.dispatch(StoreActiveGame.Actions.updateGame(response));
            const g = response.game;
            if (g.status === GameStatus.Over) {
                Controller.navigateTo(Routes.gameResults(g.id));
            }
        }

        public static onGameUpdateReceived(response : StateAndEventResponse) : void {
            const currentGame = Controller.state.activeGame.game;
            const isCurrentGame = currentGame && currentGame.id === response.game.id;
            if (isCurrentGame) {
                Controller.Game.updateInProgressGame(response);
            }
            const message = Controller.Game.getGameUpdateNotificationMessage(response, isCurrentGame);
            Controller.addNotification(StoreNotifications.NotificationType.Info, message);
        }

        private static getGameUpdateNotificationMessage(response : StateAndEventResponse, isCurrentGame : boolean) : string {
            const eventDesc = Copy.getEventDescription(response.event, response.game);
            if (isCurrentGame) {
                return eventDesc;
            } else {
                const game = response.game;
                const gameDesc = game.parameters.description;
                const gameLabel = gameDesc ? gameDesc : `Game ${game.id}`
                return `In ${gameLabel}, ${eventDesc}`;
            }
        }
    }

    public static Forms = class {
        public static updateCreateGameForm(formData : GameParameters) : void {
            Controller.dispatch(StoreCreateGameForm.Actions.updateCreateGameForm(formData))
        }

        public static updateGamesQuery(formData : GamesQuery) : void {
            Controller.dispatch(StoreGamesQuery.Actions.updateGamesQuery(formData))
        }
    }

    public static Display = class {
        public static changeTheme(themeName : string) : void {
            LocalStorageService.themeName = themeName;
            const currentTheme = Controller.state.display.theme;
            if (currentTheme && currentTheme.name === themeName) {
                return;
            }
            else {
                Controller.Display.setThemeInner(themeName);
            }
        }

        public static loadSavedTheme() : void {
            let name = LocalStorageService.themeName;
            name = name ? name : ThemeFactory.default.name;
            Controller.Display.setThemeInner(name);
        }

        private static setThemeInner(name : string) : void {
            const theme = ThemeFactory.getThemes().get(name);
            Controller.dispatch(StoreDisplay.Actions.changeTheme(theme));
            Controller.Display.applyToCss(theme);
            Controller.Display.loadThemeImages(theme);
        }

        private static applyToCss(theme : Theme) : void {
            const s = document.documentElement.style;
            const c  = theme.colors;

            s.setProperty("--background-color", c.background);
            s.setProperty("--text-color", c.text);
            s.setProperty("--header-text-color", c.headerText);
            s.setProperty("--border-color", c.border);
            s.setProperty("--hover-text-color", c.hoverText);
            s.setProperty("--hover-background-color", c.hoverBackground);
            s.setProperty("--alt-row-background-color", c.altRowBackground);
            s.setProperty("--alt-row-text-color", c.altRowText);

            s.setProperty("--player-color-0", c.players.p0);
            s.setProperty("--player-color-1", c.players.p1);
            s.setProperty("--player-color-2", c.players.p2);
            s.setProperty("--player-color-3", c.players.p3);
            s.setProperty("--player-color-4", c.players.p4);
            s.setProperty("--player-color-5", c.players.p5);
            s.setProperty("--player-color-6", c.players.p6);
            s.setProperty("--player-color-7", c.players.p7);

            s.setProperty("--font-family", theme.fonts.normalFamily);
            s.setProperty("--header-font-family", theme.fonts.headerFamily);
        }

        private static loadThemeImages(theme : Theme) : void {
            const kinds = [
                PieceKind.Assassin,
                PieceKind.Chief,
                PieceKind.Diplomat,
                PieceKind.Gravedigger,
                PieceKind.Reporter,
                PieceKind.Thug
            ];
            const maxPlayerColorId = 7;

            kinds.forEach(k => {
                for (let colorId=0; colorId<=maxPlayerColorId; colorId++) {
                    Controller.Display.createPieceImage(theme, k, colorId);
                }
                Controller.Display.createPieceImage(theme, k, null); //Neutral sprite for abandoned pieces
            });

            Controller.Display.createPieceImage(theme, PieceKind.Corpse, null); //Corpses are only ever neutral
        }

        private static createPieceImage(theme : Theme, kind : PieceKind, colorId : number) : HTMLImageElement {
            let image = Controller.state.display.images.pieces.get(kind);
            if (!image) {
                image = new (window as any).Image() as HTMLImageElement;
                image.src = ThemeService.getPieceImagePath(theme, kind);
                image.onload = () => {
                    const dummyColor = theme.colors.players.placeholder;
                    const playerColor = ThemeService.getPlayerColor(theme, colorId);
                    image = Images.replaceColor(image, dummyColor, playerColor);

                    const info : PieceImageInfo = {
                        kind: kind,
                        playerColorId: colorId,
                        image: image
                    };

                    Controller.dispatch(StoreDisplay.Actions.pieceImageLoaded(info));
                }
            }
            return image;
        }

        public static resizeBoardArea(size : Point) : void {
            const current = Controller.state.display.boardContainerSize;
            if (Geometry.Point.isCloseTo(size, current, 0.000001)) { return; }
            Controller.dispatch(StoreDisplay.Actions.boardAreaResize(size));
        }
    }

    public static addNotification(type : StoreNotifications.NotificationType, message : string) : void {
        const info : StoreNotifications.NotificationInfo = {
            id: generateQuickGuid(),
            message: message,
            type: type
        };

        const add = StoreNotifications.Actions.addNotification(info);
        Controller.dispatch(add);

        const displayMs = Controller.state.settings.debug.showNotificationsSeconds * 1000;
        setTimeout(
            () => Controller.removeNotification(info.id),
            displayMs);
    }

    public static removeNotification(id : string) : void {
        const remove = StoreNotifications.Actions.removeNotification(id);
        Controller.dispatch(remove);
    }
}