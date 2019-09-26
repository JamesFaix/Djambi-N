import Selectors from "../../selectors";
import { SectionHeader } from "../controls/headers";
import * as React from 'react';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';
import PlayerNoteIcon from '../controls/playerNoteIcon';
import * as LobbySeats from '../../viewModel/lobbySeats';
import { Seat } from '../../viewModel/lobbySeats';
import { PlayerKind } from "../../api/model";
import { TextInput } from "../controls/input";

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
export default MutablePlayersTable;

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
                <TextInput
                    value={name}
                    onChange={x => setName(x)}
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