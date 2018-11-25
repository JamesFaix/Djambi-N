import * as Model from './model';
import * as $ from 'jquery';
import Environment from "../environment";

export default class ApiClient {

    async getCurrentUser() : Promise<Model.UserResponse> {
        let result : Model.UserResponse;
        return await $.ajax({
            type : "GET",
            url: Environment.apiAddress() + "/users/current",
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Get current user succeeded");
                result = data;
            },
            error : () => {
                console.log("Get current user failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async createUser(request : Model.CreateUserRequest) : Promise<Model.UserResponse> {
        let result : Model.UserResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/users",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Create user succeeded");
                result = data;
            },
            error : () => {
                console.log("Create user failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async login(request : Model.LoginRequest) : Promise<Model.UserResponse> {
        let result : Model.UserResponse;

        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/sessions",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Create session succeeded");
                result = data;
            },
            error : () => {
                console.log("Create session failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        })
        .then(_ => result);
    }

    async logout() : Promise<void> {
        return await $.ajax({
            type : "DELETE",
            url: Environment.apiAddress() + "/sessions",
            dataType : "json",
            success: (data, status, xhr) => {
                console.log("Close session succeeded");
            },
            error : x => {
                console.log(x);
                console.log("Close session failed");
            },
            crossDomain : true,
            xhrFields : {
                withCredentials: true
            }
        });
    }
}