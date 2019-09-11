import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { User, Game, Player, PlayerStatus } from '../../api/model';
import { Classes } from '../../styles/styles';
import IconButton from '../controls/iconButton';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import { SectionHeader } from '../controls/headers';
import { Icons } from '../../utilities/icons';
import Controller from '../../controller';

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

        return (<>
            <SectionHeader text="Diplomacy"/>
            <table>
                <tbody>
                    <tr>
                        <th>Player</th>
                        <th>Status</th>
                        <th></th>
                        <th>Actions</th>
                    </tr>
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
        </>);
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
        <tr
            className={Classes.playerBox}
            data-player-color-id={p.colorId}
        >
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
                    icon={Icons.PlayerActions.acceptDraw}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.AcceptsDraw)}
                />
            : null}
            {canRevokeDraw ?
                <IconButton
                    icon={Icons.PlayerActions.revokeDraw}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.Alive)}
                />
            : null}
            {canConcede ?
                <IconButton
                    icon={Icons.PlayerActions.concede}
                    onClick={() => props.changePlayerStatus(props.gameId, p.id, PlayerStatus.Conceded)}
                />
            : null}
        </div>
    );
};

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game,
        changePlayerStatus: (gameId : number, playerId : number, status : PlayerStatus) =>
            Controller.Game.changePlayerStatus(gameId, playerId, status)
    };
}

const DiplomacyPlayersTable = connect(mapStateToProps)(diplomacyPlayersTable);
export default DiplomacyPlayersTable;