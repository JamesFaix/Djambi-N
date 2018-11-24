import * as Model from './model';
import * as $ from 'jquery';
import Environment from "../environment";

export default class Client {

    private static instance : Client = new Client();
    public static Instance() : Client { return this.instance; }

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
            }
        })
        .then(_ => result);
    }

    async login(request : Model.LoginRequest) : Promise<void> {
        return await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/sessions",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Create session succeeded");
            },
            error : () => {
                console.log("Create session failed");
            }
        });
    }
}