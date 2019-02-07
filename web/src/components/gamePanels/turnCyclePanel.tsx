import * as React from 'react';
import '../../index.css';
import { Game } from '../../api/model';
import ThemeService from '../../themes/themeService';
import { Classes, Styles } from '../../styles';

export interface TurnCyclePanelProps {
    game : Game,
    theme : ThemeService
}

export default class TurnCyclePanel extends React.Component<TurnCyclePanelProps> {

    private readonly scale = "50px";

    render() {
        const rowClass = Classes.combine([Classes.thinBorder, Classes.centerAligned]);

        return (
            <div className={Classes.thinBorder}>
                Turn Cycle
                <div className={Classes.flex}>
                    {
                        this.getPlayerViews()
                            .map((pv, i) => {
                                const style = Styles.combine([Styles.background(pv.color), Styles.height(this.scale), Styles.width(this.scale)])
                                return (
                                    <div key={"turn" + i}
                                        className={rowClass}
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