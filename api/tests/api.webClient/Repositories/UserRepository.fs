module Djambi.Api.WebClient.UserRepository

open System.Threading.Tasks
open Djambi.Api.Web.Model.LobbyWebModel
open Djambi.Api.WebClient.Model
open Djambi.Api.WebClient.WebUtility

let createUser (request : CreateUserJsonModel) : UserJsonModel Response Task =
    sendRequest(POST, "/users", 
        Some request,
        None)
    
let deleteUser (userId : int) : Unit Response Task =
    sendRequest(DELETE, sprintf "/users/%i" userId, 
        None,
        None)

let getUser (userId : int) : UserJsonModel Response Task =
    sendRequest(GET, sprintf "/users/%i" userId, 
        None,
        None)

let getUsers () : UserJsonModel list Response Task =
    sendRequest(GET, "/users", 
        None,
        None)

//Not implemented
let updateUser (request : UserJsonModel) : UserJsonModel Response Task =
    sendRequest(PATCH, sprintf "/users/%i" request.id, 
        Some request,
        None)