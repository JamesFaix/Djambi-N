import Environment from '../environment';
import { ApiRequest } from './requestModel';
import { AppStore } from '../store/root';
import * as StoreApiClient from '../store/apiClient';
import Controller from '../controllers/controller';
import { generateQuickGuid } from '../utilities/guids';
import { NotificationType } from '../store/notifications';
import { ProblemDetails } from './model';
import { mapKeys, camelCase } from 'lodash';

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

    private static createRequest(method : HttpMethod, route : string, body: any) : [ApiRequest, RequestInit] {
        const fetchParams : RequestInit = {
            method: method.toString(),
            credentials: "include",
            mode: "cors"
        };

        if (body !== null) {
            fetchParams.headers = {
                "Content-Type": "application/json",
            };
            fetchParams.body = JSON.stringify(body);
        }

        let request : ApiRequest;
        if (this.store) {
            request = {
                id: generateQuickGuid(),
                url: Environment.apiAddress() + route,
                method: method.toString(),
                body: body
            };
            const act = StoreApiClient.Actions.apiRequest(request);
            this.store.dispatch(act);
        }
        return [request, fetchParams];
    }

    private static getDefaultErrorMessage(statusCode : number) : string {
        switch (statusCode) {
            case 400: return "Bad request";
            case 401: return "Unauthorized";
            case 403: return "Forbidden";
            case 404: return "Not found";
            case 405: return "Method not allowed";
            case 409: return "Conflict";
            case 500: return "Internal server error";
            default: return `${statusCode}`;
        }
    }

    private static describe(request : ApiRequest) : string {
        return `${request.method} ${request.url}`;
    }

    private static async handleSuccess<T>(request : ApiRequest, response : Response) : Promise<T> {
        if (this.store) {
            const act = StoreApiClient.Actions.apiResponse({
                requestId: request.id,
                body: response
            });
            this.store.dispatch(act);
        }

        if (this.shouldLog()) {
            console.log(`${this.describe(request)} succeeded`);
        }

        const result = response.json();
        return result;
    }

    private static getErrorMessage(obj : any) : string {
        const camelCased = mapKeys(obj, (_, k) => camelCase(k));
        const problem = camelCased as ProblemDetails;
        if (problem.title === "One or more validation errors occurred.") {
            return Object.values(problem.errors)
                .reduce((x, y) => x.concat(y))
                .join("\n");
        }
        else {
            return problem.title;
        }
    }

    private static async handleError<T>(request : ApiRequest, response : Response) : Promise<T> {
        if (this.store){
            const act = StoreApiClient.Actions.apiError({
                requestId: request.id,
                message: ""
            });
            this.store.dispatch(act);
        }

        const status = response.status

        if (status === 401 && !request.url.endsWith('/users/current')) {
            // Redirect to login
            Controller.Session.onUnauthorized();
        }

        let message = ''
        try {
            const obj = await response.json();
            message = this.getErrorMessage(obj);
        }
        catch (ex) {
            if (this.shouldLog()) {
                console.error(ex);
            }
            message = this.getDefaultErrorMessage(status);
        }

        if (this.shouldLog()) {
            console.error(`${this.describe(request)} failed (${message})`);
        }
        Controller.addNotification(NotificationType.Error, message);
        throw [status, message]; //Rethrow so other promises chained to this don't execute
    }

    public static async sendRequest<TBody, TResponse>(
        method : HttpMethod,
        route : string,
        body : TBody = null)
        : Promise<TResponse> {

        const [request, fetchParams] = this.createRequest(method, route, body);
        const response = await fetch(request.url, fetchParams);

        if (response.ok) {
            return await this.handleSuccess(request, response);
        } else {
            return await this.handleError(request, response);
        }
    }
}