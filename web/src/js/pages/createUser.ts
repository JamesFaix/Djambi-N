import {LobbyClient} from "../apiClient/LobbyClient";
import {CreateUserRequest, SigninRequest, Role} from "../apiClient/LobbyModel";
import '../../css/global.css';
import {SessionClient} from "../apiClient/SessionClient";

class App {

    static async init() {
        document.getElementById("btn_createUser").onclick = 
            (e) => App.submitCreateUser();
            
        document.getElementById("btn_signin").onclick = 
            (e) => App.submitSignin();

        document.getElementById("btn_signout").onclick =
            (e) => App.submitSignout();
    }

    static async submitCreateUser() {
        const nameField = <HTMLInputElement>document.getElementById("input_signupName");
        const passwordField = <HTMLInputElement>document.getElementById("input_signupPassword");
        const isGuestCheckbox = <HTMLInputElement>document.getElementById("input_signupIsGuest");        

        const name = nameField.value;
        const pass = passwordField.value;
        const role = isGuestCheckbox.checked ? Role.Guest : Role.Normal;

        const request = new CreateUserRequest(name, pass, role);
        const user = await LobbyClient.createUser(request);

        nameField.value = "";
        passwordField.value = "";
        isGuestCheckbox.checked = false;
    }

    static async submitSignin() {
        const nameField = <HTMLInputElement>document.getElementById("input_signinName");       
        const passwordField = <HTMLInputElement>document.getElementById("input_signinPassword");
       
        const name = nameField.value;
        const pass = passwordField.value;

        const request = new SigninRequest(name, pass);
        await SessionClient.createSessionWithUser(request);

        nameField.value = "";
        passwordField.value = "";
    }

    static async submitSignout() {
        await SessionClient.closeSession();
    }
}

App.init();