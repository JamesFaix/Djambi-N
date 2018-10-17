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
    
let deleteUser (userId : int) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/users/%i" userId, 
        None,
        None)

let getUser (userId : int) : UserResponseJsonModel AsyncResponse =
    sendRequest(GET, sprintf "/users/%i" userId, 
        None,
        None)

let getUsers () : UserResponseJsonModel list AsyncResponse =
    sendRequest(GET, "/users", 
        None,
        None)