import * as React from 'react';
import { GameStatus, Player, PlayerKind } from '../../api/model';
import LoadGame from '../utilities/loadGame';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../containers/basicPageContainer';
import Controller from '../../controllers/controller';
import Selectors from '../../selectors';
import { SectionHeader } from '../controls/headers';
import { Classes } from '../../styles/styles';
import Copy from '../../utilities/copy';
import PlayerStatusIcon from '../controls/playerStatusIcon';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import MutablePlayersTable from '../pageSections/lobbyPageMutablePlayersTable';

const LobbyPage : React.SFC<{}> = props => {
    const routeGameId = (props as any).match.params.gameId;
    const game = Selectors.game();
    return (
        <BasicPageContainer>
            <RedirectToLoginIfNotLoggedIn/>
            <LoadGame gameId={routeGameId}/>
            {game ? <PageBody/> : null}
        </BasicPageContainer>
    );
}
export default LobbyPage;

const PageBody : React.SFC<{}> = _ => {
    const game = Selectors.game();
    return (<>
        <GameParametersTable/>
        <br/>
        <br/>
        {game.status === GameStatus.Pending
            ? <MutablePlayersTable/>
            : <PlayersTable/>
        }
        <br/>
        <br/>
        <StartButton/>
    </>);
};

const GameParametersTable : React.SFC<{}> = props => {
    const game = Selectors.game();
    if (!game) { return null; }

    return (<>
        <SectionHeader text="Game settings"/>
        <table>
            <tbody>
                <tr>
                    <td>Description</td>
                    <td>{game.parameters.description}</td>
                </tr>
                <tr>
                    <td>Regions</td>
                    <td className={Classes.centered}>
                        {game.parameters.regionCount}
                    </td>
                </tr>
                <tr>
                    <td>Allow guests</td>
                    <td className={Classes.centered}>
                        {Copy.boolToYesOrNo(game.parameters.allowGuests)}
                    </td>
                </tr>
                <tr>
                    <td>Public</td>
                    <td className={Classes.centered}>
                        {Copy.boolToYesOrNo(game.parameters.isPublic)}
                    </td>
                </tr>
                <tr>
                    <td>Status</td>
                    <td className={Classes.centered}>
                        {game.status}
                    </td>
                </tr>
            </tbody>
        </table>
    </>);
}

//#region Immutable players table

const PlayersTable : React.SFC<{}> = _ => {
    const game = Selectors.game();
    return (<>
        <SectionHeader text="Players"/>
        <table>
            <tbody>
                {game.players.map((p, i) =>
                    <PlayerRow key={i} player={p} />
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
            <td>
                <PlayerName player={p}/>
            </td>
            <td>
                <PlayerNoteIcon player={p} game={game}/>
            </td>
            <td>
                <PlayerStatusIcon player={p}/>
            </td>
        </tr>
    );
}

const PlayerName : React.SFC<{
    player : Player,
}> = props => {
    const theme = Selectors.theme();
    const player = props.player;
    const color = player.kind === PlayerKind.Neutral ? theme.colors.border : theme.colors.text;
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

//#endregion

const StartButton : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();

    const canStart = game && user &&
        game.createdBy.userId === user.id &&
        game.status === GameStatus.Pending &&
        game.players.length > 1;

    return canStart ?
        <IconButton
            icon={Icons.UserActions.startGame}
            showTitle={true}
            onClick={() => Controller.Game.startGame(game.id)}
        />
    : null;
}