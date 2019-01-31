import * as React from 'react';
import '../../index.css';
import { Game } from '../../api/model';
import ThemeService from '../../themes/themeService';

export interface TurnCyclePanelProps {
    game : Game,
    theme : ThemeService
}

export default class TurnCyclePanel extends React.Component<TurnCyclePanelProps> {

    private readonly scale = 50;

    render() {
        const containerStyle = {
            display: "flex"
        };

        return (
            <div className="thinBorder">
                Turn Cycle
                <div style={containerStyle}>
                    {
                        this.getPlayerViews()
                            .map((pv, i) => {
                                const style = {
                                    background: pv.color,
                                    height: this.scale,
                                    width: this.scale
                                };
                                return (
                                    <div key={"turn" + i}
                                        className="thinBorder centeredContainer"
                                        style={style}>
                                        {pv.id}
                                    </div>
                                );
                            })
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