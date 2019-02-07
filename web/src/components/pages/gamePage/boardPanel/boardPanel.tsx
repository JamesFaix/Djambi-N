import * as React from 'react';
import { Game } from '../../../../api/model';
import ThemeService from '../../../../themes/themeService';
import { Classes } from '../../../../styles';

export interface BoardPanelProps {
    game : Game,
    theme : ThemeService
}

export interface BoardPanelState {

}

export default class BoardPanel extends React.Component<BoardPanelProps, BoardPanelState> {
    constructor(props : BoardPanelProps) {
        super(props);
        this.state = {
        };
    }

    render() {
        return (
            <div>Board panel</div>
        );
    }
}