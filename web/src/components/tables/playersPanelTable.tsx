import * as React from 'react';
import { Game, Player, PlayerKind } from '../../api/model';
import { Kernel as K } from '../../kernel';
import PlayerStatusIcon from '../icons/playerStatusIcon';
import * as Sprintf from 'sprintf-js';
import Icon, { IconKind } from '../icons/icon';

export interface PlayersPanelTableProps {
    game : Game
}

export default class PlayersPanelTable extends React.Component<PlayersPanelTableProps> {

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

    private renderPlayerRow(player : Player, rowNumber : number) {
        const color = K.theme.getPlayerColor(player.colorId);
        return (
            <tr
                style={K.styles.playerGlow(color)}
                key={"row" + rowNumber}
            >
                {this.renderStatusCell(player)}
                {this.renderNameCell(player)}
                {this.renderGuestOfCell(player)}
            </tr>
        );
    }

    private renderStatusCell(player : Player) {
        const className = K.classes.borderless;
        const style = K.styles.combine([
            K.styles.width("5px"),
            K.styles.padRight("0px")
        ]);

        return (
            <td
                className={className}
                style={style}
            >
                <PlayerStatusIcon player={player}/>
            </td>
        );
    }

    private renderNameCell(player : Player) {
        let className = K.classes.borderless;
        if (player.kind === PlayerKind.Neutral) {
            className = K.classes.combine([className, K.classes.lightText]);
        }

        const style = K.styles.padLeft("0px");

        return (
            <td
                className={className}
                style={style}
                title={this.getPlayerHint(player)}
            >
                {player.name}
            </td>
        );
    }

    private renderGuestOfCell(player : Player) {
        const className = K.classes.borderless;

        return (
            <td
                className={className}
                title={this.getPlayerHint(player)}
            >
                {this.renderGuestIcon(player)}
            </td>
        );
    }

    private getPlayerHint(player : Player) : string {
        const note = K.copy.getPlayerNote(player, this.props.game);
        return note
            ? Sprintf.sprintf("%s (%s)", player.name, note)
            : player.name;
    }

    private renderGuestIcon(player : Player) {
        if (player.kind !== PlayerKind.Guest) {
            return undefined;
        }

        const hostColor = this.getHostPlayerColor(player);
        const style = {color: hostColor};
        return (
            <div style={style}>
                <Icon kind={IconKind.GuestOf}/>
            </div>
        );
    }

    private getHostPlayerColor(guestPlayer : Player) {
        const hostPlayer = this.props.game.players
            .find(p => p.userId === guestPlayer.userId
                    && p.kind === PlayerKind.User);

        return K.theme.getPlayerColor(hostPlayer.colorId)
    }
}