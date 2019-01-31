import * as React from 'react';
import '../../index.css';
import { Game } from '../../api/model';
import ThemeService from '../../themes/themeService';

export interface PlayersPanelProps {
    game : Game,
    theme : ThemeService
}

export default class PlayersPanel extends React.Component<PlayersPanelProps> {

    private readonly scale = 50;

    render() {
        return (
            <div className="thinBorder">
                Players
                
            </div>
        );
    }
}