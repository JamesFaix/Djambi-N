import { Dispatch } from "redux";
import { LoginRequest, CreateUserRequest, User } from "../api/model";
import * as Api from "../api/client";
import * as ModelFactory from "../api/modelFactory";
import Routes from "../routes";
import * as StoreSession from '../store/session';
import { SseClientManager } from "../utilities/serverSentEvents";
import ThemeService from "../themes/themeService";
import * as StoreGamesQuery from '../store/gamesQuery';
import Controller from "./controller";

export default class SessionStoreFlows {
    public static login(request : LoginRequest) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const session = await Api.login(request);
            dispatch(StoreSession.Actions.login(session.user));
            Controller.navigateTo(Routes.dashboard);
            return SessionStoreFlows.finishLoginSetup(session.user, dispatch);
        };
    }

    public static logout() {
        return async function (dispatch : Dispatch) : Promise<void> {
            await Api.logout();
            dispatch(StoreSession.Actions.logout());
            Controller.navigateTo(Routes.login);
            SseClientManager.disconnect();
        };
    }

    public static signup(request : CreateUserRequest) {
        return async function (dispatch : Dispatch) : Promise<void> {
            const user = await Api.createUser(request);
            dispatch(StoreSession.Actions.signup(user));
            const loginRequest = ModelFactory.loginRequestFromCreateUserRequest(request);
            return SessionStoreFlows.login(loginRequest)(dispatch);
        };
    }

    public static redirectToDashboardIfLoggedIn() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(StoreSession.Actions.restoreSession(user));
                Controller.navigateTo(Routes.dashboard);
                return SessionStoreFlows.finishLoginSetup(user, dispatch);
            }
            catch(ex) {
                let [status, message] = ex;
                if (status !== 401) {
                    throw ex;
                }
            }
        };
    }

    public static redirectToLoginIfNotLoggedIn() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(StoreSession.Actions.restoreSession(user));
                return SessionStoreFlows.finishLoginSetup(user, dispatch);
            }
            catch (ex) {
                console.log(ex);
                let [status, message] = ex;
                if (status !== 401) {
                    throw ex;
                }
                Controller.navigateTo(Routes.login);
            }
        };
    }

    public static redirectToLoginOrDashboard() {
        return async function (dispatch : Dispatch) : Promise<void> {
            try {
                const user = await Api.getCurrentUser();
                dispatch(StoreSession.Actions.restoreSession(user));
                Controller.navigateTo(Routes.dashboard);
                return SessionStoreFlows.finishLoginSetup(user, dispatch);
            }
            catch (ex) {
                console.log(ex);
                let [status, message] = ex;
                if (status === 401) {
                    Controller.navigateTo(Routes.login);
                }
                else {
                    throw ex;
                }
            }
        };
    }

    private static finishLoginSetup(user : User, dispatch : Dispatch) : Promise<void> {
        SseClientManager.connect();
        ThemeService.loadSavedTheme(dispatch);
        Controller.Settings.loadAndApply();
        return SessionStoreFlows.queryGamesForUser(user, dispatch);
    }

    private static queryGamesForUser(user: User, dispatch: Dispatch) : Promise<void> {
        const query = ModelFactory.emptyGamesQuery();
        query.playerUserName = user.name;
        dispatch(StoreGamesQuery.Actions.updateGamesQuery(query));
        return Controller.queryGames(query);
    }
}