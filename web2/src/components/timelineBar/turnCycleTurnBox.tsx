import * as React from 'react';
import { Player } from '../../api/model';
import Colors from '../../utilities/colors';

interface TurnCycleTurnBoxProps {
    player : Player
}

export default class TurnCycleTurnBox extends React.Component<TurnCycleTurnBoxProps> {
    render() {
        const p = this.props.player;
        const color = Colors.getColorFromPlayerColorId(p.colorId);
        const style : React.CSSProperties = {
            borderStyle: "solid",
            borderWidth: "thin",
            borderColor: "gainsboro",
            padding: "10px",
            boxShadow: `inset 0 0 3px ${color}`
        };
        return (
            <div style={style}>
                {p.name}
            </div>
        );
    }
}