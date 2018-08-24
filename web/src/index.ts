import {VisualBoardFactory} from "./display/VisualBoardFactory";
import {LobbyClient} from "./apiClient/LobbyClient";
import {Renderer} from "./display/Renderer";
import {ClickHandler} from "./display/ClickHandler";
import {GameCreationRequest} from "./apiClient/LobbyModel";
import { VisualBoard } from "./display/VisualBoard";
import { GameStartResponse, TurnState } from "./apiClient/PlayModel";
import {PlayClient} from "./apiClient/PlayClient";
import { ITheme, ThemeFactory } from "./display/Theme";

class App {
    private static renderer : Renderer;
    private static gameId : number;

    static async createGame() : Promise<void> {
        const canvas = <HTMLCanvasElement>document.getElementById("canvas");
        const request = this.getFormData();
        const lobbyGame = await LobbyClient.createGame(request);
        
        this.gameId = lobbyGame.id;
        
        const board = await this.createBoard(request.boardRegionCount, this.gameId);                
        const startResponse = await LobbyClient.startGame(this.gameId);
        this.displayGame(canvas, board, startResponse);
    }

    private static getFormData() : GameCreationRequest {
        const regionCount = <number><any>((<HTMLInputElement>document.getElementById("input_regionCount")).value);
        const description = (<HTMLInputElement>document.getElementById("input_description")).value;
        return new GameCreationRequest(regionCount, description);
    }

    private static async createBoard(regionCount : number, gameId : number) : Promise<VisualBoard> {
        const cellSize = Math.floor(160 * Math.pow(Math.E, (-0.2 * regionCount)));
        return await VisualBoardFactory.createBoard(regionCount, cellSize, gameId);
    }

    private static displayGame(
        canvas : HTMLCanvasElement, 
        board : VisualBoard, 
        startResponse : GameStartResponse) : void {
        
        canvas.getContext("2d").clearRect(0, 0, canvas.width, canvas.height);
        if (this.renderer) {
            this.renderer.clearPieces();
        }

        this.renderer = new Renderer(canvas, board, startResponse.startingConditions);
        this.renderer.theme = this.getSelectedTheme();
        const clickHandler = new ClickHandler(this.renderer, canvas, board);    

        canvas.onclick = (e) => clickHandler.clickOnBoard(e);
        this.renderer.onPieceClicked = (gameId, cellId) => clickHandler.clickOnCell(gameId, cellId);
        this.renderer.updateGame(startResponse.gameState, startResponse.turnState);
    }

    static async resetTurn() {
        if (this.gameId) {
            const turn = await PlayClient.resetTurn(this.gameId);
            this.renderer.updateTurn(turn);
        }
    }

    static async commitTurn() {
        if (this.gameId) {
            const response = await PlayClient.commitTurn(this.gameId);
            this.renderer.updateGame(response.gameState, response.turnState);
        }
    }

    static initializeThemeDropDown() {
        const dropDown = <HTMLSelectElement>document.getElementById("select_theme");
        ThemeFactory.getThemeNames()
            .forEach(name => {
                const option = <HTMLOptionElement>document.createElement("option");
                option.text = name;
                dropDown.add(option);
            });

        dropDown.onchange = (e) => {            
            this.renderer.theme = this.getSelectedTheme();
            this.renderer.refresh();
        }
    }

    static getSelectedTheme() : ITheme {
        const dropDown = <HTMLSelectElement>document.getElementById("select_theme");
        const value = dropDown.options[dropDown.selectedIndex].value;
        return ThemeFactory.getTheme(value);  
    }
}

document.getElementById("btn_createGame").onclick = 
    (e) => App.createGame();

document.getElementById("btn_turnReset").onclick =
    (e) => App.resetTurn();

document.getElementById("btn_turnConfirm").onclick =
    (e) => App.commitTurn();

App.initializeThemeDropDown();