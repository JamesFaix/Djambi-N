import * as React from 'react';
import '../../index.css';
import { Game } from '../../api/model';
import ThemeService from '../../themes/themeService';
import StyleService from '../../styleService';

export interface TurnCyclePanelProps {
    game : Game,
    theme : ThemeService
}

export default class TurnCyclePanel extends React.Component<TurnCyclePanelProps> {

    private readonly scale = 50;

    render() {
        const rowClass = StyleService.classes([StyleService.classThinBorder, StyleService.classCenteredContainer]);

        return (
            <div className={StyleService.classThinBorder}>
                Turn Cycle
                <div style={StyleService.styleFlexContainer()}>
                    {
                        this.getPlayerViews()
                            .map((pv, i) =>
                                <div key={"turn" + i}
                                    className={rowClass}
                                    style={StyleService.turnCycleElement(pv.color, this.scale)}>
                                    {pv.id}
                                </div>
                            )
                    }
                </div>
            </div>
        );
    }

    private getPlayerViews() : PlayerView[] {
        const game = this.props.game;

        return game.turnCycle
            .map(pId => {
                const p = game.players.find(p => p.id === pId);
                return {
                    id: p.id,
                    name: p.name,
                    color: this.props.theme.getPlayerColor(p.colorId)
                }
            });
    }
}

interface PlayerView {
    id : number,
    name : string,
    color : string,
}