module Djambi.Api.Web.Managers.UserManager

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Common
open Djambi.Api.Model

let createUser (request : CreateUserRequest) (sessionOption : Session option) : User AsyncHttpResult =
    UserService.createUser request sessionOption
    |> thenMap UserDetails.hideDetails

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    UserService.deleteUser userId session

let getUser (userId : int) (session : Session) : User AsyncHttpResult =
    UserService.getUser userId session
    |> thenMap UserDetails.hideDetails

let getUsers (session : Session) : User list AsyncHttpResult =
    UserService.getUsers session
    |> thenMap (List.map UserDetails.hideDetails)

let getCurrentUser (session : Session) : User AsyncHttpResult =
    UserService.getUser session.userId session
    |> thenMap UserDetails.hideDetails