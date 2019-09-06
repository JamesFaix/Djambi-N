import * as React from 'react';
import { Game, Effect, Board } from "../../api/model";
import Copy from '../../utilities/copy';

interface GameHistoryEffectBoxProps {
    game : Game,
    effect : Effect,
    board : Board
}

const GameHistoryEffectBox : React.SFC<GameHistoryEffectBoxProps> = props => {
    return (
        <div>
            {Copy.getEffectDescription(props.effect, props.game, props.board)}
        </div>
    );
}

export default GameHistoryEffectBox;