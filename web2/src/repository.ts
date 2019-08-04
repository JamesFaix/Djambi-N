import ApiClient from "./api/client";
import { Store } from "redux";
import { LoginRequest, CreateUserRequest, EventsQuery, ResultsDirection } from "./api/model";
import { ActionFactory, CustomAction } from "./store/actions";
import { AppState } from "./store/state";

export class Repository {
    private readonly api : ApiClient;
    private readonly store : Store<AppState, CustomAction>;
    private readonly actionFactory : ActionFactory;

    constructor(
        api: ApiClient,
        store : Store<AppState, CustomAction>,
        actionFactory : ActionFactory
    ){
        this.api = api;
        this.store = store;
        this.actionFactory = actionFactory;
    }

    login(request : LoginRequest) : void {
        let a = this.actionFactory.loginRequest(request);
        this.store.dispatch(a);
        this.api.login(request)
            .then(session => {
                let a = this.actionFactory.loginSuccess(session);
                this.store.dispatch(a);
            })
            .catch(_ => {
                let a = this.actionFactory.loginError();
                this.store.dispatch(a);
            });
    }

    logout() : void {
        let a = this.actionFactory.logoutRequest();
        this.store.dispatch(a);
        this.api.logout()
            .then(_ => {
                let a = this.actionFactory.logoutSuccess();
                this.store.dispatch(a);
            })
            .catch(_ => {
                let a = this.actionFactory.logoutError();
                this.store.dispatch(a);
            });
    }

    signup(request: CreateUserRequest) : void {
        let a = this.actionFactory.signupRequest(request);
        this.store.dispatch(a);
        this.api.createUser(request)
            .then(user => {
                let a = this.actionFactory.signupSuccess(user);
                this.store.dispatch(a);
            })
            .catch(_ => {
                let a = this.actionFactory.signupError();
                this.store.dispatch(a);
            });
    }

    loadGame(gameId : number) : void {
        let a = this.actionFactory.loadGameRequest(gameId);
        this.store.dispatch(a);
        this.api.getGame(gameId)
            .then(game => {
                let a = this.actionFactory.loadGameSuccess(game);
                this.store.dispatch(a);
            })
            .catch(_ => {
                let a = this.actionFactory.loadGameError();
                this.store.dispatch(a);
            });
    }

    loadGameHistory(gameId : number) : void {
        let a = this.actionFactory.loadGameHistoryRequest(gameId);
        let query : EventsQuery =
        {
            maxResults: null,
            direction: ResultsDirection.Ascending,
            thresholdEventId: null,
            thresholdTime: null
        };
        this.store.dispatch(a);
        this.api.getEvents(gameId, query)
            .then(events => {
                let a = this.actionFactory.loadGameHistorySuccess(events);
                this.store.dispatch(a);
            })
            .catch(_ => {
                let a = this.actionFactory.loadGameHistoryError();
                this.store.dispatch(a);
            });
    }
}