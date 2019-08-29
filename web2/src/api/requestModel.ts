export interface ApiRequest {
    id : string,
    url : string,
    method : string,
    body : any
}

export interface ApiResponse {
    requestId : string,
    body : any
}

export interface ApiError {
    requestId : string,
    message : string
}