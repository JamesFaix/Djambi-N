import * as React from 'react';
import { State as AppState } from '../../store/root';
import { connect } from 'react-redux';
import { Board } from '../../api/model';
import { Theme } from '../../themes/model';
import BoardSelector from './boardSelector';

const boardSelectionBar : React.SFC<{
    boards : Map<number, Board>,
    theme : Theme,
    selectBoard : (regionCount : number) => void
}> = props => {
    const types = [3, 4, 5, 6, 7, 8];
    const size = 100;
    const border = 5;

    const [activeRegionCount, setActiveRegionCount] = React.useState(3);

    return (
        <div style={{
            display: "flex",
        }}>
            {types.map(n => {
                const b = props.boards.get(n);
                if (!b) {
                    return null;
                }
                return (
                    <BoardSelector
                        key={n}
                        isSelected={activeRegionCount === n}
                        onClick={() => {
                            props.selectBoard(n);
                            setActiveRegionCount(n);
                        }}
                        board={b}
                        size={size}
                        theme={props.theme}
                        borderSize={border}
                    />
                );
            })}
        </div>
    );
}

const mapStateToProps = (state : AppState) => {
    return {
        boards : state.boards.boards,
        theme : state.display.theme
    };
}

const BoardSelectionBar = connect(mapStateToProps)(boardSelectionBar);
export default BoardSelectionBar;