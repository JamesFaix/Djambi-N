import * as React from 'react';
import { State as AppState } from '../../store/root';
import { connect } from 'react-redux';
import { Board } from '../../api/model';
import { Theme } from '../../themes/model';
import BoardSelector from './boardSelector';

interface Props {
    boards : Map<number, Board>,
    theme : Theme,
    selectBoard : (regionCount : number) => void
}

interface State {
    activeRegionCount : number
}

class boardSelectionBar extends React.Component<Props, State> {
    constructor(props : Props) {
        super(props);
        this.state = { activeRegionCount: 3 };
    }

    render(){
        const types = [3, 4, 5, 6, 7, 8];
        const size = 100;
        const border = 5;
        return (
            <div style={{
                display: "flex",
            }}>
                {types.map(n => {
                    const b = this.props.boards.get(n);
                    if (!b) {
                        return null;
                    }
                    return (
                        <BoardSelector
                            key={n}
                            isSelected={this.state.activeRegionCount === n}
                            onClick={() => this.onClick(n)}
                            board={b}
                            size={size}
                            theme={this.props.theme}
                            borderSize={border}
                        />
                    );
                })}
            </div>
        );
    }

    private onClick(regionCount : number) {
        this.props.selectBoard(regionCount);
        this.setState({ activeRegionCount: regionCount });
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        boards : state.boards.boards,
        theme : state.display.theme
    };
}

const BoardSelectionBar = connect(mapStateToProps)(boardSelectionBar);
export default BoardSelectionBar;