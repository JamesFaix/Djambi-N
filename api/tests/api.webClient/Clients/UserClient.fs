module Djambi.Api.WebClient.UserClient

open Djambi.Api.Model
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createUser (request : CreateUserRequest) : User AsyncResponse =
    sendRequest(POST, "/users",
        Some request,
        None)

let tryCreateUserWithToken (request : CreateUserRequest, token : string) : User AsyncResponse =
    sendRequest(POST, "/users",
        Some request,
        Some token)

let deleteUser (userId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/users/%i" userId,
        None,
        Some token)

let getUser (userId : int, token : string) : User AsyncResponse =
    sendRequest(GET, sprintf "/users/%i" userId,
        None,
        Some token)

let getUsers (token : string) : User list AsyncResponse =
    sendRequest(GET, "/users",
        None,
        Some token)