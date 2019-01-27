import * as React from 'react';
import ApiClient from '../api/client';
import { User, Game } from '../api/model';

 export interface GameBoardProps {
    user : User,
    api : ApiClient,
    game : Game
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
            <div>
                        Game board
                <canvas id="board">

                </canvas>
            </div>
        );
    }
}