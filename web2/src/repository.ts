import ApiClient from "./api/client";
import { Store } from "redux";
import { LoginRequest, CreateUserRequest } from "./api/model";
import { CustomAction } from "./store/actions";
import * as Actions from "./store/actions";
import { AppState } from "./store/state";

export class Repository {
    private readonly api : ApiClient;
    private readonly store : Store<AppState, CustomAction>;

    constructor(
        api: ApiClient,
        store : Store<AppState, CustomAction>,
    ){
        this.api = api;
        this.store = store;
    }

    login(request : LoginRequest) : void {
        this.store.dispatch(Actions.loginRequest(request));
        this.api.login(request)
            .then(session => {
                this.store.dispatch(Actions.loginSuccess(session));
            })
            .catch(_ => {
                this.store.dispatch(Actions.loginError());
            });
    }

    logout() : void {
        this.store.dispatch(Actions.logoutRequest());
        this.api.logout()
            .then(_ => {
                this.store.dispatch(Actions.logoutSuccess());
            })
            .catch(error => {

                this.store.dispatch(Actions.logoutError());
            });
    }

    signup(request: CreateUserRequest) : void {
        this.store.dispatch(Actions.signupRequest(request));
        this.api.createUser(request)
            .then(async user => {
                this.store.dispatch(Actions.signupSuccess(user));
            })
            .catch(_ => {
                this.store.dispatch(Actions.signupError());
            })
            .then(_ => {
                const loginRequest : LoginRequest = {
                    username: request.name,
                    password: request.password
                };
                return this.login(loginRequest);
            });
    }
}