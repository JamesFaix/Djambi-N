import * as React from 'react';
import PlayersPanelTable from '../tables/playersPanelTable';
import { Game } from '../../api/model';
import { Kernel as K } from '../../kernel';
import Icon, { IconKind } from '../icons/icon';

export interface PlayersPanelProps {
    game : Game,
    height ?: string,
    width : string
}

export default class PlayersPanel extends React.Component<PlayersPanelProps> {
    render() {
        let style = K.styles.combine([
            K.styles.flex(0),
            K.styles.width(this.props.width)
        ]);
        if (this.props.height) {
            style = K.styles.combine([style, K.styles.height(this.props.height)]);
        }

        return (
            <div className={K.classes.thinBorder} style={style}>
                <Icon
                    kind={IconKind.Players}
                    hint="Players"
                />
                <PlayersPanelTable
                    game={this.props.game}
                />
            </div>
        );
    }
}