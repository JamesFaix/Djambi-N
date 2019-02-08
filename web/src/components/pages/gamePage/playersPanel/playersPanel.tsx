import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import PlayersPanelTable from './playersPanelTable';
import { Classes, Styles } from '../../../../styles';

export interface PlayersPanelProps {
    game : Game,
    theme : ThemeService,
    height : string,
    width : string
}

export default class PlayersPanel extends React.Component<PlayersPanelProps> {
    render() {
        const style = Styles.combine([Styles.height(this.props.height), Styles.width(this.props.width)]);
        return (
            <div className={Classes.thinBorder} style={style}>
                Players
                <PlayersPanelTable
                    game={this.props.game}
                    theme={this.props.theme}
                />
            </div>
        );
    }
}