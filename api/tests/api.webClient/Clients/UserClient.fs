module Djambi.Api.WebClient.UserClient

open Djambi.Api.Web.Model.UserWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createUser (request : CreateUserJsonModel) : UserResponseJsonModel AsyncResponse =
    sendRequest(POST, "/users", 
        Some request,
        None)

let tryCreateUserWithToken (request : CreateUserJsonModel, token : string) : UserResponseJsonModel AsyncResponse =
    sendRequest(POST, "/users",
        Some request,
        Some token)
    
let deleteUser (userId : int, token : string) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/users/%i" userId, 
        None,
        Some token)

let getUser (userId : int, token : string) : UserResponseJsonModel AsyncResponse =
    sendRequest(GET, sprintf "/users/%i" userId, 
        None,
        Some token)

let getUsers (token : string) : UserResponseJsonModel list AsyncResponse =
    sendRequest(GET, "/users", 
        None,
        Some token)