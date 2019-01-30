import * as React from 'react';
import ApiClient from '../api/client';
import { User, Game } from '../api/model';
import BoardRenderer from '../boardRendering/boardRenderer';
import ThemeService from '../themes/themeService';
import { BoardView } from '../boardRendering/model';

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
            </div>
        );
    }

    componentDidUpdate() {
        if (this.props.game && this.props.boardView) {
            const div = document.getElementById("div_board") as HTMLDivElement;
            const offset = { x: div.offsetLeft, y: div.offsetTop };
            const renderer = new BoardRenderer(offset, this.props.boardView, this.props.theme);
            renderer.updateGame(this.props.game);
        }
    }
}