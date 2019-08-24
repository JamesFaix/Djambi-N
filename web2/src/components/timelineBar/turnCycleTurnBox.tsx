import * as React from 'react';
import { Player } from '../../api/model';
import Styles from '../../styles/styles';

interface TurnCycleTurnBoxProps {
    player : Player
}

export default class TurnCycleTurnBox extends React.Component<TurnCycleTurnBoxProps> {
    render() {
        const p = this.props.player;
        const style = Styles.playerBoxGlow(p.colorId);
        return (
            <div style={style}>
                {p.name}
            </div>
        );
    }
}