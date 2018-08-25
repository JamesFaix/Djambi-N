import {LobbyClient} from "../apiClient/LobbyClient";
import {LobbyGame, User, CreateUserRequest} from "../apiClient/LobbyModel";
import '../../css/global.css';

class App {

    static async init() {
        document.getElementById("btn_createUser").onclick = 
            (e) => App.submitForm();
    }

    static async submitForm() {
        const nameField = <HTMLInputElement>document.getElementById("input_name");
        const isGuestCheckbox = <HTMLInputElement>document.getElementById("input_isGuest");        

        const name = nameField.value;
        const isGuest = isGuestCheckbox.checked;            

        const request = new CreateUserRequest(name, isGuest);
        const user = await LobbyClient.createUser(request);

        nameField.value = "";
        isGuestCheckbox.checked = false;
    }
}

App.init();