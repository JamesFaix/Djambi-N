import * as React from 'react';
import { Game, Player } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { SectionHeader } from '../controls/headers';

interface PlayersTableProps {
    game: Game
}

class playersTable extends React.Component<PlayersTableProps> {
    render() {
        const g = this.props.game;
        const cellStyle = {
            borderStyle: "none"
        };
        return (
            <div>
                <SectionHeader text="Players"/>
                <table>
                    <tbody>
                        {g.players
                            .map((p, i) => this.renderRow(g, p, i, cellStyle))
                        }
                    </tbody>
                </table>
            </div>
        );
    }

    private renderRow(game: Game, player: Player, rowNumber: number, style : React.CSSProperties) {
        return (
            <tr key={rowNumber}>
                <td style={style}>{player.name}</td>
                <td style={style}>{LobbySeats.getPlayerNote(player, game)}</td>
                <td style={style}>{player.status}</td>
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