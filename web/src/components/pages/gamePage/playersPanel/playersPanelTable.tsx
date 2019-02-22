import * as React from 'react';
import { Game, Player } from '../../../../api/model';
import {Kernel as K} from '../../../../kernel';

export interface PlayersPanelTableProps {
    game : Game
}

export default class PlayersPanelTable extends React.Component<PlayersPanelTableProps> {

    private renderPlayerRow(player : Player, rowNumber : number) {
        const color = K.theme.getPlayerColor(player.colorId);
        return (
            <tr
                style={K.styles.playerGlow(color)}
                key={"row" + rowNumber}
            >
                <td className={K.classes.borderless}>
                    {player.name}
                </td>
                <td className={K.classes.combine([K.classes.borderless, K.classes.lightText])}>
                    {K.copy.getPlayerNote(player, this.props.game)}
                </td>
            </tr>
        );
    }

    render() {
        return (
            <div className={K.classes.flex}>
                <table className={K.classes.combine([K.classes.table, K.classes.fullWidth])}>
                    <tbody>
                        {
                            this.props.game.players
                                .map((p, i) => this.renderPlayerRow(p, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }
}