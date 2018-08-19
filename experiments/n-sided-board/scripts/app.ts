import {Renderer} from "./display/Renderer.js";
import {VisualBoardFactory} from "./display/VisualBoardFactory.js";
import {LobbyClient} from "./apiClient/LobbyClient.js";

class App {
    static async main() : Promise<void> {
        for (var i = 3; i <= 8; i++) {
            const cellSize = Math.floor(160 * Math.pow(Math.E, (-0.2 * i)));
    
            const canvas = <HTMLCanvasElement>document.getElementById("canvas" + i);
            const lobbyGame = await LobbyClient.createGame(i);
            const startResponse = await LobbyClient.startGame(lobbyGame.id);
            const board = await VisualBoardFactory.createBoard(i, cellSize, lobbyGame.id);
            
            Renderer.drawBoard(board, canvas);
            Renderer.drawPieces(board, canvas, startResponse);    
        }
    }
}

window.onload = App.main;
