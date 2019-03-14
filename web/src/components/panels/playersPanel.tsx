import * as React from 'react';
import PlayersPanelTable from '../tables/playersPanelTable';
import { Game } from '../../api/model';
import { Kernel as K } from '../../kernel';

export interface PlayersPanelProps {
    game : Game,
    height : string,
    width : string
}

export default class PlayersPanel extends React.Component<PlayersPanelProps> {
    render() {
        const style = K.styles.combine([K.styles.height(this.props.height), K.styles.width(this.props.width)]);
        return (
            <div className={K.classes.thinBorder} style={style}>
                Players
                <PlayersPanelTable
                    game={this.props.game}
                />
            </div>
        );
    }
}