import * as React from 'react';
import { Game } from '../../../api/model';
import ThemeService from '../../../themes/themeService';
import { Classes, Styles } from '../../../styles';

export interface TurnCyclePanelProps {
    game : Game,
    theme : ThemeService,
    iconSize : string,
    height : string,
    width : string
}

export default class TurnCyclePanel extends React.Component<TurnCyclePanelProps> {
    render() {
        const rowClass = Classes.combine([Classes.thinBorder, Classes.centerAligned]);
        const style = Styles.combine([Styles.height(this.props.height), Styles.width(this.props.width)]);

        return (
            <div className={Classes.thinBorder} style={style}>
                Turn Cycle
                <div className={Classes.flex}>
                    {
                        this.getPlayerViews()
                            .map((pv, i) => {
                                const style = Styles.combine([
                                    Styles.playerGlow(pv.color),
                                    Styles.height(this.props.iconSize),
                                    Styles.width(this.props.iconSize)
                                ]);
                                return (
                                    <div key={"turn" + i}
                                        className={rowClass}
                                        style={style}>
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