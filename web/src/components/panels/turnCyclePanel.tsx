import * as React from 'react';
import { Game } from '../../api/model';
import { Kernel as K } from '../../kernel';
import Icon, { IconKind } from '../icons/icon';

interface PlayerView {
    id : number,
    name : string,
    color : string,
}

export interface TurnCyclePanelProps {
    game : Game,
    iconSize : string,
    height? : string,
    width : string
}

export default class TurnCyclePanel extends React.Component<TurnCyclePanelProps> {
    public render() : JSX.Element {
        const rowClass = K.classes.combine([K.classes.thinBorder, K.classes.centerAligned]);
        let style = K.styles.combine([
            K.styles.flex(0),
            K.styles.width(this.props.width)
        ]);
        if (this.props.height) {
            style = K.styles.combine([style, K.styles.height(this.props.height)]);
        }

        return (
            <div className={K.classes.thinBorder} style={style}>
                <div className={K.classes.flex}>
                    <Icon
                        kind={IconKind.TurnCycle}
                        hint="Turn cycle"
                    />
                    {
                        this.getPlayerViews()
                            .map((pv, i) => this.renderCell(i, pv, rowClass))
                    }
                </div>
            </div>
        );
    }

    private renderCell(rowNumber : number, playerView : PlayerView, rowClass : string) {
        const style = K.styles.combine([
            K.styles.playerGlow(playerView.color),
            K.styles.height(this.props.iconSize),
            K.styles.width(this.props.iconSize)
        ]);
        return (
            <div key={"turn" + rowNumber}
                className={rowClass}
                style={style}
                title={playerView.name}
            >
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