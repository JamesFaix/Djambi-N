import * as $ from 'jquery';
import {Environment} from "../Environment";
import {SigninRequest} from "./LobbyModel";

export class SessionClient {
    constructor() {

    }

    static async createSessionWithUser(request : SigninRequest) : Promise<void> {

        await $.ajax({
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

    static async addUserToSession(request : SigninRequest) : Promise<void> {

        await $.ajax({
            type : "POST",
            url: Environment.apiAddress() + "/sessions/users",
            dataType : "json",
            data: request,
            success: (data, status, xhr) => {
                console.log("Add user to session succeeded");
            },
            error : () => {
                console.log("Add user to session failed");
            }
        });
    }

    static async removeUserFromSession(userId : number) : Promise<void> {

        await $.ajax({
            type : "DELETE",
            url: Environment.apiAddress() + "/sessions/users/" + userId,
            success: (data, status, xhr) => {
                console.log("Remove user from session succeeded");
            },
            error : () => {
                console.log("Remove user from session failed");
            }
        });
    }
    
    static async closeSession() : Promise<void> {

        await $.ajax({
            type : "DELETE",
            url: Environment.apiAddress() + "/sessions",
            success: (data, status, xhr) => {
                console.log("Close session succeeded");
            },
            error : () => {
                console.log("Close session failed");
            }
        });
    }
}