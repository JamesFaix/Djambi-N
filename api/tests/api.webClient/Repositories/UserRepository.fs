module Djambi.Api.WebClient.UserRepository

open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createUser (request : CreateUserJsonModel) : UserJsonModel AsyncResponse =
    sendRequest(POST, "/users", 
        Some request,
        None)
    
let deleteUser (userId : int) : Unit AsyncResponse =
    sendRequest(DELETE, sprintf "/users/%i" userId, 
        None,
        None)

let getUser (userId : int) : UserJsonModel AsyncResponse =
    sendRequest(GET, sprintf "/users/%i" userId, 
        None,
        None)

let getUsers () : UserJsonModel list AsyncResponse =
    sendRequest(GET, "/users", 
        None,
        None)

//Not implemented
let updateUser (request : UserJsonModel) : UserJsonModel AsyncResponse =
    sendRequest(PATCH, sprintf "/users/%i" request.id, 
        Some request,
        None)