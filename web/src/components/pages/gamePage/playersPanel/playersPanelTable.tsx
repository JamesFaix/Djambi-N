import * as React from 'react';
import { Game, Player, PlayerKind } from '../../../../api/model';
import {Kernel as K} from '../../../../kernel';

export interface PlayersPanelTableProps {
    game : Game
}

export default class PlayersPanelTable extends React.Component<PlayersPanelTableProps> {

    private getPlayerNote(player : Player) {
        switch (player.kind) {
            case PlayerKind.User:
                return "";

            case PlayerKind.Guest:
                const host = this.props.game.players
                    .find(p => p.userId === player.userId
                        && p.kind === PlayerKind.User);

                return "Guest of " + host.name;

            case PlayerKind.Neutral:
                return "Neutral";

            default:
                throw "Invalid player kind.";
        }
    }

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
                    {this.getPlayerNote(player)}
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