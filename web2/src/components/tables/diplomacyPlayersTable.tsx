import * as React from 'react';
import { AppState } from '../../store/state';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { User, Game, Player, PlayerStatus } from '../../api/model';
import Styles from '../../styles/styles';
import IconButton from '../controls/iconButton';
import { faHandshake, faHandMiddleFinger, faFlag } from '@fortawesome/free-solid-svg-icons';
import * as ThunkActions from '../../thunkActions';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import { SectionHeader } from '../controls/headers';

interface DiplomacyPageProps {
    user : User,
    game : Game
    changePlayerStatus : (gameId : number, playerId : number, status : PlayerStatus) => void
}

class diplomacyPlayersTable extends React.Component<DiplomacyPageProps>{
    render() {
        if (!this.props.game) {
            return null;
        }

        const user = this.props.user;
        const players = this.props.game.players.filter(p => p.userId === user.id);

        return (
            <div>
                <SectionHeader text="Diplomatic actions"/>
                <table>
                    <tbody>
                        {players.map((p, i) =>
                            <PlayerRow
                                game={this.props.game}
                                player={p}
                                changePlayerStatus={this.props.changePlayerStatus}
                                key={i}
                            />
                        )}
                    </tbody>
                </table>
            </div>
        )
    }
}

interface PlayerRowProps {
    game : Game,
    player : Player,
    changePlayerStatus : (gameId : number, playerId : number, status : PlayerStatus) => void
}

const PlayerRow : React.SFC<PlayerRowProps> = props => {
    const p = props.player;
    return (
        <tr style={Styles.playerBoxGlow(props.player.colorId)}>
            <td>{p.name}</td>
            <td>
                <PlayerStatusIcon
                    player={p}
                />
            </td>
            <td>
                <PlayerNoteIcon
                    player={p}
                    game={props.game}
                />
            </td>
            <td>
                <PlayerDiplomacyActionButtons
                    gameId={props.game.id}
                    player={p}
                    changePlayerStatus={props.changePlayerStatus}
                />
            </td>
        </tr>
    );
};

interface PlayerDiplomacyActionButtonsProps {
    gameId : number,
    player : Player,
    changePlayerStatus : (gameId : number, playerId : number, status : PlayerStatus) => void
}

const PlayerDiplomacyActionButtons : React.SFC<PlayerDiplomacyActionButtonsProps> = props => {
    const p = props.player;
    const s = p.status;

    const canConcede = s === PlayerStatus.Alive || s === PlayerStatus.AcceptsDraw;
    const canAcceptDraw = s === PlayerStatus.Alive;
    const canRevokeDraw = s === PlayerStatus.AcceptsDraw;

    return (
        <div>
            {canAcceptDraw ?
                <IconButton
                    title="Accept draw"
                    icon={faHandshake}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.AcceptsDraw)}
                />
            : null}
            {canRevokeDraw ?
                <IconButton
                    title="Revoke draw"
                    icon={faHandMiddleFinger}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.Alive)}
                />
            : null}
            {canConcede ?
                <IconButton
                    title="Concede"
                    icon={faFlag}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.Conceded)}
                />
            : null}
        </div>
    );
};

const mapStateToProps = (state : AppState) => {
    return {
        user: state.session.user,
        game: state.activeGame.game
    };
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        changePlayerStatus: (gameId : number, playerId : number, status : PlayerStatus) =>
            ThunkActions.changePlayerStatus(gameId, playerId, status)(dispatch)
    };
}

const DiplomacyPlayersTable = connect(mapStateToProps, mapDispatchToProps)(diplomacyPlayersTable);
export default DiplomacyPlayersTable;