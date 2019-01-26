module Djambi.Api.Logic.Managers.UserManager

open Djambi.Api.Common.Control  
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model
open Djambi.ClientGenerator.Annotations

[<ClientFunction(HttpMethod.Post, "/users")>]
let createUser (request : CreateUserRequest) (sessionOption : Session option) : User AsyncHttpResult =
    UserService.createUser request sessionOption
    |> thenMap UserDetails.hideDetails

[<ClientFunction(HttpMethod.Delete, "/users/%i")>]
let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    UserService.deleteUser userId session
    
[<ClientFunction(HttpMethod.Get, "/users/%i")>]
let getUser (userId : int) (session : Session) : User AsyncHttpResult =
    UserService.getUser userId session
    |> thenMap UserDetails.hideDetails
    
[<ClientFunction(HttpMethod.Get, "/users")>]
let getUsers (session : Session) : User list AsyncHttpResult =
    UserService.getUsers session
    |> thenMap (List.map UserDetails.hideDetails)
    
[<ClientFunction(HttpMethod.Get, "/users/current")>]
let getCurrentUser (session : Session) : User AsyncHttpResult =
    UserService.getUser session.userId session
    |> thenMap UserDetails.hideDetails