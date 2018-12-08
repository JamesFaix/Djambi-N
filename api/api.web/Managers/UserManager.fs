module Djambi.Api.Web.Managers.UserManager

open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Web.Mappings.UserWebMapping
open Djambi.Api.Web.Model.UserWebModel
open Djambi.Api.Common
open Djambi.Api.Model.SessionModel

let createUser (jsonModel : CreateUserJsonModel) (sessionOption : Session option) : UserResponseJsonModel AsyncHttpResult =
    let model = mapCreateUserRequest jsonModel
    UserService.createUser model sessionOption
    |> thenMap mapUserResponse

let deleteUser (userId : int) (session : Session) : Unit AsyncHttpResult =
    UserService.deleteUser userId session

let getUser (userId : int) (session : Session) : UserResponseJsonModel AsyncHttpResult =
    UserService.getUser userId session
    |> thenMap mapUserResponse

let getUsers (session : Session) : UserResponseJsonModel list AsyncHttpResult =
    UserService.getUsers session
    |> thenMap (List.map mapUserResponse)

let getCurrentUser (session : Session) : UserResponseJsonModel AsyncHttpResult =
    UserService.getUser session.userId session
    |> thenMap mapUserResponse