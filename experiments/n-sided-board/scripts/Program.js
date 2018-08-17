var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { Renderer } from "./display/Renderer.js";
import { ClickHandler } from "./display/ClickHandler.js";
import { VisualBoardFactory } from "./display/VisualBoardFactory.js";
import { LobbyClient } from "./apiClient/LobbyClient.js";
export class Program {
    static main() {
        return __awaiter(this, void 0, void 0, function* () {
            for (var i = 3; i <= 3; i++) {
                const cellSize = Math.floor(160 * Math.pow(Math.E, (-0.2 * i)));
                const canvas = document.getElementById("canvas" + i);
                const board = yield VisualBoardFactory.createBoard(i, cellSize);
                Renderer.drawBoard(board, canvas);
                canvas.onclick = function (e) {
                    ClickHandler.logClickOnBoard(e, board, canvas);
                };
            }
            let lobbyGame = yield LobbyClient.createGame(3);
            let currenState = yield LobbyClient.startGame(lobbyGame.id);
        });
    }
}
//# sourceMappingURL=Program.js.map