import * as React from 'react';
import ApiClient from '../api/client';
import { User, Game } from '../api/model';
import BoardView from '../display/boardView';
import BoardRenderer from '../display/boardRenderer';

export interface GameBoardProps {
    user : User,
    api : ApiClient,
    game : Game,
    boardView : BoardView
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
            const renderer = new BoardRenderer(canvas, this.props.boardView);
            renderer.updateGame(this.props.game);
        }
    }
}