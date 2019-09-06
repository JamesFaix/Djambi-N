import * as React from 'react';
import { Game, Player, PlayerKind } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { SectionHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import { Theme } from '../../themes/model';

interface PlayersTableProps {
    game : Game,
    theme : Theme
}

class playersTable extends React.Component<PlayersTableProps> {
    render() {
        const g = this.props.game;
        return (<>
            <SectionHeader text="Players"/>
            <table>
                <tbody>
                    {g.players.map((p, i) =>
                        <PlayerRow
                            player={p}
                            game={g}
                            key={i}
                            theme={this.props.theme}
                        />
                    )}
                </tbody>
            </table>
        </>);
    }
}

interface PlayerRowProps {
    game : Game,
    player : Player,
    theme : Theme
}

class PlayerRow extends React.Component<PlayerRowProps> {
    render() {
        const player = this.props.player;
        return (
            <tr
                className={Classes.playerBox}
                data-player-color-id={player.colorId}
            >
                <td>
                    <PlayerName player={player} theme={this.props.theme}/>
                </td>
                <td>
                    <PlayerNoteIcon player={player} game={this.props.game}/>
                </td>
                <td>
                    <PlayerStatusIcon player={player}/>
                </td>
            </tr>
        );
    }
}

interface PlayerNameProps {
    player : Player,
    theme : Theme,
}

class PlayerName extends React.Component<PlayerNameProps> {
    render(){
        const player = this.props.player;
        const color = player.kind === PlayerKind.Neutral ? this.props.theme.colors.border : this.props.theme.colors.text;
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
        game: state.activeGame.game,
        theme: state.display.theme
    };
};

const PlayersTable = connect(mapStateToProps)(playersTable);

export default PlayersTable;