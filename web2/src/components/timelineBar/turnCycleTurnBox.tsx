import * as React from 'react';
import { Player } from '../../api/model';
import { Classes } from '../../styles/styles';

interface TurnCycleTurnBoxProps {
    player : Player
}

export default class TurnCycleTurnBox extends React.Component<TurnCycleTurnBoxProps> {
    render() {
        const p = this.props.player;
        return (
            <div
                className={Classes.playerBox}
                data-player-color-id={p.colorId}
            >
                {p.name}
            </div>
        );
    }
}