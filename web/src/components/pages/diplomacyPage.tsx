import * as React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../containers/basicPageContainer';
import { GameStatus } from '../../api/model';
import { Player, PlayerStatus } from '../../api/model';
import { Classes } from '../../styles/styles';
import IconButton from '../controls/iconButton';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import { SectionHeader } from '../controls/headers';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';

const DiplomacyPage : React.SFC<{}> = props => {
    const i = Icons.PlayerActions;
    const routeGameId = (props as any).match.params.gameId;

    React.useEffect(() => {
        Controller.Session.redirectToLoginIfNotLoggedIn()
        .then(() => Controller.Game.loadGameIfNotLoaded(routeGameId))
        .then(() => Controller.Game.redirectToLobbyIfGameNotStatus(GameStatus.InProgress));
    });

    return (
        <BasicPageContainer>
            <DiplomacyPlayersTable/>
            <br/>
            <p>
                If a player concedes (<FontAwesomeIcon icon={i.concede.icon}/>),
                they are removed from the turn cycle and all their pieces are abandoned. This cannot be undone.
                If a player concedes when it is not their turn, it does not take effect until their next turn would start.
            </p>
            <br/>
            <p>
                If all players who have not conceded or been eliminated accept a draw (<FontAwesomeIcon icon={i.acceptDraw.icon}/>),
                the game ends and no one wins. If you have accepted a draw, but not everyone has, you can revoke your acceptance at any time (<FontAwesomeIcon icon={i.revokeDraw.icon}/>).
            </p>
        </BasicPageContainer>
    );
}
export default DiplomacyPage;


const DiplomacyPlayersTable : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();

    //Must put early terminations after use of React hooks
    if (!game) { return null; }

    const players = game.players.filter(p => p.userId === user.id);

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
                        player={p}
                        key={i}
                    />
                )}
            </tbody>
        </table>
    </>);
}

const PlayerRow : React.SFC<{
    player : Player,
}> = props => {
    const game = Selectors.game();
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
                    game={game}
                />
            </td>
            <td>
                <PlayerDiplomacyActionButtons
                    player={p}
                />
            </td>
        </tr>
    );
};

const PlayerDiplomacyActionButtons : React.SFC<{
    player : Player,
}> = props => {
    const game = Selectors.game();
    const p = props.player;
    const s = p.status;

    const canConcede = s === PlayerStatus.Alive || s === PlayerStatus.AcceptsDraw;
    const canAcceptDraw = s === PlayerStatus.Alive;
    const canRevokeDraw = s === PlayerStatus.AcceptsDraw;
    const onClick = Controller.Game.changePlayerStatus;

    return (
        <div>
            {canAcceptDraw ?
                <IconButton
                    icon={Icons.PlayerActions.acceptDraw}
                    onClick={() => onClick(game.id, p.id, PlayerStatus.AcceptsDraw)}
                />
            : null}
            {canRevokeDraw ?
                <IconButton
                    icon={Icons.PlayerActions.revokeDraw}
                    onClick={() => onClick(game.id, p.id, PlayerStatus.Alive)}
                />
            : null}
            {canConcede ?
                <IconButton
                    icon={Icons.PlayerActions.concede}
                    onClick={() => onClick(game.id, p.id, PlayerStatus.Conceded)}
                />
            : null}
        </div>
    );
};