import * as React from 'react';
import { Game, Effect, Board } from "../../api/model";
import * as Copy from '../../utilities/copy';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { Theme } from '../../themes/model';
import { DebugSettings } from '../../debug';

interface GameHistoryEffectBoxProps {
    game : Game,
    effect : Effect,
    board : Board,
    theme : Theme,
    debugSettings : DebugSettings
}

const gameHistoryEffectBox : React.SFC<GameHistoryEffectBoxProps> = props => {
    return (
        <div>
            {Copy.getEffectDescription(props.theme, props.effect, props.game, props.board, props.debugSettings.showCellAndPieceIds)}
        </div>
    );
}

const mapStateToProps = (state : State) => {
    return {
        theme: state.display.theme,
        debugSettings: state.settings.debug
    };
}

const GameHistoryEffectBox = connect(mapStateToProps)(gameHistoryEffectBox);
export default GameHistoryEffectBox;