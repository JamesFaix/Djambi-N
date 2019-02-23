import Environment from '../environment';

export enum HttpMethod {
    Get = "GET",
    Put = "PUT",
    Post = "POST",
    Delete = "DELETE"
}

export class ApiClientCore {

    public static async sendRequest<TBody, TResponse>(
        method : HttpMethod,
        route : string,
        body : TBody = null)
        : Promise<TResponse> {

        const url = Environment.apiAddress() + route;
        const endpointDescription = method.toString() + " " + route

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

        return await fetch(url, fetchParams)
            .then(async response => {
                if (!response.ok){
                    return response.json()
                        .then(errorMessage => {
                            console.log(endpointDescription + " failed (" + errorMessage + ")");
                            return null;

                        });
                } else {
                    console.log(endpointDescription + " succeeded");
                    return await response.json();
                }
            })
            .catch(reason => {
                console.log(reason);
            });
    }
}