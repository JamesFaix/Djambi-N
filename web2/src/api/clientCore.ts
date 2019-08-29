import Environment from '../environment';
import Debug from '../debug';
import { ApiRequest, ApiResponse, ApiError } from './requestModel';

export enum HttpMethod {
    Get = "GET",
    Put = "PUT",
    Post = "POST",
    Delete = "DELETE"
}

function generateQuickGuid() {
    return Math.random().toString(36).substring(2, 15) +
        Math.random().toString(36).substring(2, 15);
}

export class ApiClientCore {
    private static onRequest : (request: ApiRequest) => void;
    private static onResponse : (response: ApiResponse) => void;
    private static onError : (error: ApiError) => void;

    public static init(
        onRequest : (request: ApiRequest) => void,
        onResponse : (response : ApiResponse) => void,
        onError : (error : ApiError) => void
    ) : void {
        if (!onRequest || !onResponse || !onError) {
            throw "Must initialize all callbacks at once.";
        }

        this.onRequest = onRequest;
        this.onResponse = onResponse;
        this.onError = onError;
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
        if (this.onRequest) {
            request = {
                id: generateQuickGuid(),
                url: url,
                method: method.toString(),
                body: body
            };
            this.onRequest(request);
        }

        return fetch(url, fetchParams)
            .then(response => {
                if (!response.ok){
                    if (this.onError){
                        this.onError({
                            requestId: request.id,
                            message: ""
                        });
                    }

                    return response.json()
                        .then(errorMessage => {
                            if (Debug.logApi) {
                                console.log(endpointDescription + " failed (" + errorMessage + ")");
                            }
                            throw [response.status, errorMessage];
                        });
                } else {
                    if (this.onResponse) {
                        this.onResponse({
                            requestId: request.id,
                            body: response
                        });
                    }

                    if (Debug.logApi) {
                        console.log(endpointDescription + " succeeded");
                    }
                    return response.json();
                }
            });
    }
}