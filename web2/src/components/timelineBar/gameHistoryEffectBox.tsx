import * as React from 'react';
import { Game, Effect, Board } from "../../api/model";
import * as Copy from '../../utilities/copy';
import { State } from '../../store/root';
import { connect } from 'react-redux';

interface GameHistoryEffectBoxProps {
    game : Game,
    effect : Effect,
    board : Board,
    theme : Theme
}

const gameHistoryEffectBox : React.SFC<GameHistoryEffectBoxProps> = props => {
    return (
        <div>
            {Copy.getEffectDescription(props.theme, props.effect, props.game, props.board)}
        </div>
    );
}

const mapStateToProps = (state : State) => {
    return {
        theme: state.display.theme
    };
}

const GameHistoryEffectBox = connect(mapStateToProps)(gameHistoryEffectBox);
export default GameHistoryEffectBox;