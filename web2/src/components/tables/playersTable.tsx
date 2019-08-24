import * as React from 'react';
import { Game, Player, PlayerStatus, PlayerKind } from '../../api/model';
import { AppState } from '../../store/state';
import { connect } from 'react-redux';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { SectionHeader } from '../controls/headers';
import Styles from '../../styles/styles';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { IconDefinition, faHandshake, faFlag, faSkull, faTrophy, faHeart, faSpinner, faIdBadge } from '@fortawesome/free-solid-svg-icons';
import Colors from '../../utilities/colors';

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

interface PlayerNoteIconProps {
    player : Player,
    game : Game
}

class PlayerNoteIcon extends React.Component<PlayerNoteIconProps> {
    render() {
        const isGuest = this.props.player.kind === PlayerKind.Guest;
        if (!isGuest) {
            return null;
        }

        const hostPlayer = this.props.game.players
            .find(p => p.userId === this.props.player.userId
                && p.kind === PlayerKind.User);

        const color = Colors.getColorFromPlayerColorId(hostPlayer.colorId);
        const note = LobbySeats.getPlayerNote(this.props.player, this.props.game);

        return (
            <FontAwesomeIcon
                icon={faIdBadge}
                style={{color: color}}
                title={note}
            />
        );
    }
}

interface PlayerStatusIconProps {
    player : Player
}

interface PlayerIconDetails {
    hint : string,
    isActive : boolean,
    icon : IconDefinition
}

class PlayerStatusIcon extends React.Component<PlayerStatusIconProps> {
    render() {
        const details = this.getIconDetails(this.props.player);
        const style = {
            color: details.isActive ? "black" : "gray"
        };
        return (
            <FontAwesomeIcon
                icon={details.icon}
                style={style}
                title={details.hint}
            />
        );
    }

    private getIconDetails(player : Player) : PlayerIconDetails {
        switch (player.status) {
            case PlayerStatus.Pending:
                return {
                    hint: "Pending",
                    icon: faSpinner,
                    isActive: false
                };
            case PlayerStatus.Alive:
                return {
                    hint: "Alive",
                    icon: faHeart,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.AcceptsDraw:
                return {
                    hint: "Will accept draw",
                    icon: faHandshake,
                    isActive: player.kind !== PlayerKind.Neutral
                };
            case PlayerStatus.Conceded:
                return {
                    hint: "Conceded",
                    icon: faFlag,
                    isActive: false
                };
            case PlayerStatus.WillConcede:
                return {
                    hint: "Will concede at the start of next turn",
                    icon: faFlag,
                    isActive: true
                };
            case PlayerStatus.Eliminated:
                return {
                    hint: "Eliminated",
                    icon: faSkull,
                    isActive: false
                };
            case PlayerStatus.Victorious:
                return {
                    hint: "Victorious",
                    icon: faTrophy,
                    isActive: true
                };
            default:
                throw "Invalid player status.";
        }
    }
}

const mapStateToProps = (state : AppState) => {
    return {
        game: state.activeGame.game
    };
};

const PlayersTable = connect(mapStateToProps)(playersTable);

export default PlayersTable;