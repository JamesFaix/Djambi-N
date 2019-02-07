import * as React from 'react';
import '../../../../index.css';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import PlayersPanelTable from './playersPanelTable';
import { Classes } from '../../../../styles';

export interface PlayersPanelProps {
    game : Game,
    theme : ThemeService
}

export default class PlayersPanel extends React.Component<PlayersPanelProps> {
    render() {
        return (
            <div className={Classes.thinBorder}>
                Players
                <PlayersPanelTable
                    game={this.props.game}
                    theme={this.props.theme}
                />
            </div>
        );
    }
}