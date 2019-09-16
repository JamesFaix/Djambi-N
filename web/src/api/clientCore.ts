import Environment from '../environment';
import { ApiRequest } from './requestModel';
import { AppStore } from '../store/root';
import * as StoreApiClient from '../store/apiClient';
import Controller from '../controllers/controller';
import { generateQuickGuid } from '../utilities/guids';

export enum HttpMethod {
    Get = "GET",
    Put = "PUT",
    Post = "POST",
    Delete = "DELETE"
}

export class ApiClientCore {
    private static store : AppStore;

    public static init(store : AppStore) : void {
        ApiClientCore.store = store;
    }

    private static shouldLog() : boolean {
        return this.store && this.store.getState().settings.debug.logApi;
    }

    public static sendRequest<TBody, TResponse>(
        method : HttpMethod,
        route : string,
        body : TBody = null)
        : Promise<TResponse> {

        const url = Environment.apiAddress() + route;
        const endpointDescription = method.toString() + " " + route;

        const fetchParams : RequestInit = {
            method: method.toString(),
            credentials: "include",
            mode: "cors"
        };

        if (body !== null) {
            fetchParams.headers=  {
                "Content-Type": "application/json",
            };
            fetchParams.body = JSON.stringify(body);
        }

        let request : ApiRequest;
        if (this.store) {
            request = {
                id: generateQuickGuid(),
                url: url,
                method: method.toString(),
                body: body
            };
            const act = StoreApiClient.Actions.apiRequest(request);
            this.store.dispatch(act);
        }

        return fetch(url, fetchParams)
            .then(response => {
                if (response.ok) {
                    if (this.store) {
                        const act = StoreApiClient.Actions.apiResponse({
                            requestId: request.id,
                            body: response
                        });
                        this.store.dispatch(act);
                    }

                    if (this.shouldLog()) {
                        console.log(endpointDescription + " succeeded");
                    }
                    return response.json();
                } else {
                    if (this.store){
                        const act = StoreApiClient.Actions.apiError({
                            requestId: request.id,
                            message: ""
                        });
                        this.store.dispatch(act);
                    }

                    if (response.status === 401 && route !== '/users/current') {
                        Controller.Session.onUnauthorized();
                    }

                    return response.json()
                        .then(errorMessage => {
                            if (this.shouldLog()) {
                                console.log(endpointDescription + " failed (" + errorMessage + ")");
                            }
                            Controller.logError(errorMessage);
                            throw [response.status, errorMessage];
                        });
                }
            });
    }
}