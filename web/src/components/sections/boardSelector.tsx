import * as React from 'react';
import { Board } from '../../api/model';
import BoardThumbnail from '../canvas/boardThumbnail';
import { Theme } from '../../themes/model';

const BoardSelector : React.SFC<{
    isSelected : boolean,
    onClick : () => void,
    board : Board,
    size : number,
    theme : Theme,
    borderSize : number
}> = props => (
    <div style={{
        display: "flex",
        flexDirection: "column",
        alignContent: "center"
    }}>
        <button
            data-active={props.isSelected}
            onClick={() => props.onClick()}
        >
            <BoardThumbnail
                board={props.board}
                size={{ x: props.size, y: props.size }}
                strokeWidth={props.borderSize}
                theme={props.theme}
            />
        </button>
        <div style={{
            textAlign: "center"
        }}>
            {props.board.regionCount}
        </div>
    </div>
);
export default BoardSelector;