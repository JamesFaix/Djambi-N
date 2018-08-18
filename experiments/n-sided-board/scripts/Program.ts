import {Renderer} from "./display/Renderer.js";
import {ClickHandler} from "./display/ClickHandler.js";
import {VisualBoardFactory} from "./display/VisualBoardFactory.js";
import {LobbyClient} from "./apiClient/LobbyClient.js";
import { Board } from "./apiClient/BoardModel.js";

export class Program {
    static async main() : Promise<void> {
        for (var i = 3; i <= 8; i++) {
            const cellSize = Math.floor(160 * Math.pow(Math.E, (-0.2 * i)));
    
            const canvas = <HTMLCanvasElement>document.getElementById("canvas" + i);
            const board = await VisualBoardFactory.createBoard(i, cellSize);
            
            Renderer.drawBoard(board, canvas);
            canvas.onclick = function(e) {
                ClickHandler.logClickOnBoard(e, board, canvas);
            };

            let lobbyGame = await LobbyClient.createGame(i);
            let currentState = await LobbyClient.startGame(lobbyGame.id);
            Renderer.drawPieces(board, canvas, currentState);    
        }
    }
}