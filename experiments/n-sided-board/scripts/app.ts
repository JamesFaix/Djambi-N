import {VisualBoardFactory} from "./display/VisualBoardFactory.js";
import {LobbyClient} from "./apiClient/LobbyClient.js";
import {Renderer} from "./display/Renderer.js";
import {ClickHandler} from "./display/ClickHandler.js";

class App {
    static async main() : Promise<void> {
        for (var i = 3; i <= 3; i++) {
            const cellSize = Math.floor(160 * Math.pow(Math.E, (-0.2 * i)));
    
            const canvas = <HTMLCanvasElement>document.getElementById("canvas" + i);

            const lobbyGame = await LobbyClient.createGame(i);
            const board = await VisualBoardFactory.createBoard(i, cellSize, lobbyGame.id);
            const startResponse = await LobbyClient.startGame(lobbyGame.id);
            const renderer = new Renderer(canvas, board, startResponse.startingConditions);
            const clickHandler = new ClickHandler(renderer, canvas, board);    

            canvas.onclick = (e) => clickHandler.clickOnBoard(e);
            renderer.onPieceClicked = (gameId, cellId) => clickHandler.clickOnCell(gameId, cellId);
            renderer.updateGame(startResponse.gameState, startResponse.turnState);
        }
    }
}

window.onload = App.main;
