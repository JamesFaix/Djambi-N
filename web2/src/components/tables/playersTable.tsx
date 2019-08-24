import * as React from 'react';
import { Game, Player } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { SectionHeader } from '../controls/headers';
import Styles from '../../styles/styles';

interface PlayersTableProps {
    game: Game
}

class playersTable extends React.Component<PlayersTableProps> {
    render() {
        const g = this.props.game;
        return (
            <div>
                <SectionHeader text="Players"/>
                <table>
                    <tbody>
                        {g.players
                            .map((p, i) => this.renderRow(g, p, i))
                        }
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(game: Game, player: Player, rowNumber: number) {
        const rowStyle = Styles.playerBoxGlow(player.colorId);
        const cellStyle = {
            borderStyle: "none"
        };

        return (
            <tr key={rowNumber} style={rowStyle}>
                <td style={cellStyle}>{player.name}</td>
                <td style={cellStyle}>{LobbySeats.getPlayerNote(player, game)}</td>
                <td style={cellStyle}>{player.status}</td>
            </tr>
        );
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game
    };
};

const PlayersTable = connect(mapStateToProps)(playersTable);

export default PlayersTable;