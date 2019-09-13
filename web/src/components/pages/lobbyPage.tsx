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
import * as LobbySeats from '../../viewModel/lobbySeats';
import { Seat } from '../../viewModel/lobbySeats';
import HtmlInputTypes from '../htmlInputTypes';

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

//#region Mutable players table

const MutablePlayersTable : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();
    if (!game) { return null; }

    const seats = LobbySeats.getSeats(game, user);
    return (<>
        <SectionHeader text="Players"/>
        <table>
            <tbody>
                {seats.map((s, i) => {
                    return <MutablePlayersTableRow key={i} seat={s}/>;
                })}
            </tbody>
        </table>
    </>);
}

const MutablePlayersTableRow : React.SFC<{
    seat : Seat
}> = props => {
    const seat = props.seat;

    switch (seat.action) {
        case LobbySeats.SeatActionType.None:
            return <NoActionRow seat={seat}/>;
        case LobbySeats.SeatActionType.Join:
            return <JoinRow/>;
        case LobbySeats.SeatActionType.AddGuest:
            return <AddGuestRow/>;
        case LobbySeats.SeatActionType.Remove:
            return <RemoveRow seat={seat}/>;
        default:
            throw "Invalid SeatActionType";
    }
}

const JoinRow : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();
    return (
        <tr>
            <td>(Empty)</td>
            <td></td>
            <td>
                <IconButton
                    icon={Icons.UserActions.playerJoin}
                    onClick={() => Controller.Game.addPlayer(game.id, {
                        userId: user.id,
                        name: null,
                        kind: PlayerKind.User
                    })}
                />
            </td>
        </tr>
    );
}

const AddGuestRow : React.SFC<{}> = _ => {
    const game = Selectors.game();
    const user = Selectors.user();
    const [name, setName] = React.useState("");
    return (
        <tr>
            <td>
                <input
                    type={HtmlInputTypes.Text}
                    value={name}
                    onChange={e => setName(e.target.value)}
                />
            </td>
            <td></td>
            <td>
                <IconButton
                    icon={Icons.UserActions.playerAddGuest}
                    onClick={() => {
                        Controller.Game.addPlayer(game.id, {
                            userId: user.id,
                            name: name,
                            kind: PlayerKind.Guest
                        });
                        setName("");
                    }}
                />
            </td>
        </tr>
    );
}

const RemoveRow : React.SFC<{
    seat : Seat
}> = props => {
    const game = Selectors.game();
    const user = Selectors.user();
    const seat = props.seat;
    const icon = LobbySeats.isSeatSelf(seat, user)
        ? Icons.UserActions.playerQuit
        : Icons.UserActions.playerRemove;

    return (
        <tr>
            <td>{seat.player.name}</td>
            <td>
                <PlayerNoteIcon
                    player={seat.player}
                    game={game}
                />
            </td>
            <td>
                <IconButton
                    icon={icon}
                    onClick={() => Controller.Game.removePlayer(game.id, seat.player.id)}
                />
            </td>
        </tr>
    );
}

const NoActionRow : React.SFC<{
    seat : Seat
}> = props => {
    const game = Selectors.game();
    const seat = props.seat;

    const playerName = seat.player
        ? seat.player.name
        : "(Empty)";

    return (
        <tr>
            <td>{playerName}</td>
            <td>
                <PlayerNoteIcon
                    player={seat.player}
                    game={game}
                />
            </td>
            <td></td>
        </tr>
    );
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