import * as React from 'react';
import '../../index.css';
import { Game } from '../../api/model';
import ThemeService from '../../themes/themeService';
import PlayersPanelTable from './playersPanelTable';
import StyleService from '../../styleService';

export interface PlayersPanelProps {
    game : Game,
    theme : ThemeService
}

export default class PlayersPanel extends React.Component<PlayersPanelProps> {
    render() {
        return (
            <div className={StyleService.classThinBorder}>
                Players
                <PlayersPanelTable
                    game={this.props.game}
                    theme={this.props.theme}
                />
            </div>
        );
    }
}