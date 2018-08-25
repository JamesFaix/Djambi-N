import {LobbyClient} from "./../apiClient/LobbyClient";
import {LobbyGame, User} from "./../apiClient/LobbyModel";
import '../../css/global.css';

class App {

    static async init() {
        const users = await LobbyClient.getUsers();
        this.fillUsersTable(users);

        const games = await LobbyClient.getGames();
        this.fillGamesTable(games);
    }

    private static fillUsersTable(users : Array<User>) {
        const table = <HTMLTableElement>document.getElementById("table_users");
        const rows = table.getElementsByTagName("tr");
        
        for (var i = rows.length-1; i > 0; i--){
            const r = rows.item(i);
            table.removeChild(r);
        }

        users.forEach(u => this.addUserRow(u));
    }

    private static addUserRow(user : User) {
        const table = <HTMLTableElement>document.getElementById("tableBody_users");

        const row = <HTMLTableRowElement>document.createElement("tr");
        const idCell = <HTMLTableDataCellElement>document.createElement("td");
        const nameCell = <HTMLTableDataCellElement>document.createElement("td");
        const isGuestCell = <HTMLTableDataCellElement>document.createElement("td");
        const isAdminCell = <HTMLTableDataCellElement>document.createElement("td");

        idCell.innerHTML = user.id + "";
        nameCell.innerHTML = user.name;
        isGuestCell.innerHTML = user.isGuest + "";
        isAdminCell.innerHTML = user.isAdmin + "";

        row.appendChild(idCell);
        row.appendChild(nameCell);
        row.appendChild(isGuestCell);
        row.appendChild(isAdminCell);

        table.appendChild(row);
    }

    private static fillGamesTable(games : Array<LobbyGame>) {
        const table = <HTMLTableElement>document.getElementById("table_games");
        const rows = table.getElementsByTagName("tr");
        
        for (var i = rows.length-1; i > 0; i--){
            const r = rows.item(i);
            table.removeChild(r);
        }

        games.forEach(g => this.addGameRow(g));
    }

    private static addGameRow(game : LobbyGame) {
        const table = <HTMLTableElement>document.getElementById("tableBody_games");

        const row = <HTMLTableRowElement>document.createElement("tr");
        const idCell = <HTMLTableDataCellElement>document.createElement("td");
        const descriptionCell = <HTMLTableDataCellElement>document.createElement("td");
        const statusCell = <HTMLTableDataCellElement>document.createElement("td");
        const playerCountCell = <HTMLTableDataCellElement>document.createElement("td");

        idCell.innerHTML = game.id + "";
        descriptionCell.innerHTML = game.description;
        statusCell.innerHTML = game.status.toString();
        playerCountCell.innerHTML = game.players.length + "";

        row.appendChild(idCell);
        row.appendChild(descriptionCell);
        row.appendChild(statusCell);
        row.appendChild(playerCountCell);

        table.appendChild(row);
    }
}

App.init();