import {VisualBoard} from "./display/VisualBoard.js";
import {Renderer} from "./display/Renderer.js";
import {ClickHandler} from "./display/ClickHandler.js";

window.onload = function() { 

    for (var i = 3; i <= 8; i++) {
        const cellSize = Math.floor(160 * Math.pow(Math.E, (-0.2 * i)));

        const canvas = document.getElementById("canvas" + i);
        const board = new VisualBoard(i, cellSize, 9);

        Renderer.drawBoard(board, canvas);
        canvas.onclick = function(e) {
            ClickHandler.logClickOnBoard(e, board, canvas);
        };
    }
}