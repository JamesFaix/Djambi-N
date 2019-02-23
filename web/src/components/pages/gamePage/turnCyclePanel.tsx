import * as React from 'react';
import { Game } from '../../../api/model';
import { Kernel as K } from '../../../kernel';

export interface TurnCyclePanelProps {
    game : Game,
    iconSize : string,
    height : string,
    width : string
}

export default class TurnCyclePanel extends React.Component<TurnCyclePanelProps> {
    render() {
        const rowClass = K.classes.combine([K.classes.thinBorder, K.classes.centerAligned]);
        const style = K.styles.combine([K.styles.height(this.props.height), K.styles.width(this.props.width)]);

        return (
            <div className={K.classes.thinBorder} style={style}>
                Turn Cycle
                <div className={K.classes.flex}>
                    {
                        this.getPlayerViews()
                            .map((pv, i) => {
                                const style = K.styles.combine([
                                    K.styles.playerGlow(pv.color),
                                    K.styles.height(this.props.iconSize),
                                    K.styles.width(this.props.iconSize)
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
                    color: K.theme.getPlayerColor(p.colorId)
                }
            });
    }
}

interface PlayerView {
    id : number,
    name : string,
    color : string,
}