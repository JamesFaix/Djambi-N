import * as React from 'react';
import { Game, Player, PlayerKind } from '../../../../api/model';
import TextCell from '../../../tables/textCell';
import ThemeService from '../../../../themes/themeService';
import { Classes, Styles } from '../../../../styles';

export interface PlayersPanelTableProps {
    game : Game,
    theme : ThemeService
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
        const color = this.props.theme.getPlayerColor(player.colorId);
        return (
            <tr
                style={Styles.playerGlow(color)}
                key={"row" + rowNumber}
            >
                <TextCell text={player.name}/>
                <TextCell text={this.getPlayerNote(player)}/>
            </tr>
        );
    }

    render() {
        return (
            <div className={Classes.flex}>
                <table className={Classes.combine([Classes.table, Classes.fullWidth])}>
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