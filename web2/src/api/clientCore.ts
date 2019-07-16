import Environment from '../environment';
import Debug from '../debug';

export enum HttpMethod {
    Get = "GET",
    Put = "PUT",
    Post = "POST",
    Delete = "DELETE"
}

export class ApiClientCore {

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

        return fetch(url, fetchParams)
            .then(response => {
                if (!response.ok){
                    return response.json()
                        .then(errorMessage => {
                            if (Debug.logApiErrors) {
                                console.log(endpointDescription + " failed (" + errorMessage + ")");
                            }
                            return null;

                        });
                } else {
                    if (Debug.logApiSuccesses) {
                        console.log(endpointDescription + " succeeded");
                    }
                    return response.json();
                }
            })
            .catch(reason => {
                console.log(reason);
            });
    }
}