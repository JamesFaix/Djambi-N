import * as React from 'react';
import { Game, Player, PlayerStatus, PlayerKind } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import Styles from '../../styles/styles';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';

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
                        {g.players.map((p, i) =>
                            <PlayerRow
                                player={p}
                                game={g}
                                key={i}
                            />
                        )}
                    </tbody>
                </table>
            </div>
        );
    }
}

interface PlayerRowProps {
    game : Game,
    player : Player
}

class PlayerRow extends React.Component<PlayerRowProps> {
    render() {
        const player = this.props.player;
        const rowStyle = Styles.playerBoxGlow(player.colorId);
        const cellStyle = {
            borderStyle: "none"
        };

        return (
            <tr style={rowStyle}>
                <td style={cellStyle}>
                    <PlayerName player={player}/>
                </td>
                <td style={cellStyle}>
                    <PlayerNoteIcon player={player} game={this.props.game}/>
                </td>
                <td style={cellStyle}>
                    <PlayerStatusIcon player={player}/>
                </td>
            </tr>
        );
    }
}

interface PlayerNameProps {
    player : Player
}

class PlayerName extends React.Component<PlayerNameProps> {
    render(){
        const player = this.props.player;
        const color = player.kind === PlayerKind.Neutral ? "gray" : "black";
        const hint = player.kind === PlayerKind.Neutral ? "Neutral" : "";
        return (
            <div
                title={hint}
                style={{color:color}}
            >
                {player.name}
            </div>
        )
    }
}

const mapStateToProps = (state : State) => {
    return {
        game: state.activeGame.game
    };
};

const PlayersTable = connect(mapStateToProps)(playersTable);

export default PlayersTable;