import * as React from 'react';
import ApiClient from '../api/client';
import { User, Game } from '../api/model';
import BoardView from '../boardRendering/boardView';
import BoardRenderer from '../boardRendering/boardRenderer';
import { BoardClickHandler } from '../boardRendering/boardClickHandler';
import ThemeService from '../themes/themeService';

export interface GameBoardProps {
    user : User,
    api : ApiClient,
    game : Game,
    boardView : BoardView,
    theme : ThemeService
}

export interface GameBoardState {
}

export default class GameBoard extends React.Component<GameBoardProps, GameBoardState> {
    constructor(props : GameBoardProps) {
        super(props);
        this.state = {
        };
    }

    render() {
        return (
            <div id="div_board" className="centeredContainer">
                <canvas
                    id="canvas_board"
                    className="board"
                    width="1000"
                    height="1000"
                />
            </div>
        );
    }

    componentDidUpdate() {
        if (this.props.game && this.props.boardView) {
            const canvas = document.getElementById("canvas_board") as HTMLCanvasElement;
            const renderer = new BoardRenderer(canvas, this.props.boardView, this.props.theme);
            const clickHandler = new BoardClickHandler(renderer, canvas, this.props.boardView, this.props.api, this.props.game.id);
            canvas.onclick = (e) => clickHandler.clickOnBoard(e);
            renderer.onPieceClicked = (gameId, cellId) => clickHandler.clickOnCell(gameId, cellId);
            renderer.updateGame(this.props.game);
        }
    }
}