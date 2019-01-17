import * as React from 'react';
import { User, Game, Player, PlayerKind, CreatePlayerRequest, GameStatus } from '../api/model';
import ActionButton from './actionButton';
import ApiClient from '../api/client';

export interface GamePlayersTableProps {
    api : ApiClient,
    user : User,
    game : Game,
    updateGame(game: Game) : void
}

export interface GamePlayersTableState {
    guestName : string
}

enum SeatActionType {
    None,
    Remove,
    Join,
    AddGuest
}

interface Seat {
    player : Player,
    note : string,
    action : SeatActionType
}

export default class GamePlayersTable extends React.Component<GamePlayersTableProps, GamePlayersTableState> {
    constructor(props : GamePlayersTableProps) {
        super(props);
        this.state = {
            guestName: ""
        };
    }

    private getSeats(game : Game) : Seat[] {
        if (game === null) {
            return [];
        }

        const self = this.props.user;

        //Get player seats first
        const seats = game.players
            .map(p => {
                console.log(p);

                const seat : Seat = {
                    player : p,
                    note : null,
                    action : SeatActionType.None
                };

                if (p.kind === PlayerKind.User
                    && p.userId === game.createdByUserId) {
                    seat.note = "Host";
                }

                if (p.kind === PlayerKind.Guest) {
                    const host = game.players
                        .find(h => h.userId === p.userId
                            && h.kind === PlayerKind.User);

                    seat.note = "Guest of " + host.name;
                }

                if (self.isAdmin
                    || game.createdByUserId === self.id
                    || seat.player.name === self.name
                    || (seat.player.kind === PlayerKind.Guest
                        && seat.player.userId === self.id)) {

                    seat.action = SeatActionType.Remove;
                }
                console.log(seat);
                return seat;
            });

        //If self is not a player, add "Join" seat
        if (!this.isSelfAPlayer(game)) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.Join
            })
        //If self is a player and guests allowed, add "Add Guest" seat
        } else if (game.parameters.allowGuests) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.AddGuest
            });
        }

        //Add empty seats until regionCount
        while (seats.length < game.parameters.regionCount) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.None
            });
        }

        return seats;
    }

    private isSelfAPlayer(game : Game) : boolean {
        return game.players.find(p => p.userId === this.props.user.id) !== undefined;
    }

    private isSeatSelf(seat : Seat) : boolean {
        return seat.player.kind === PlayerKind.User
            && seat.player.userId === this.props.user.id;
    }

    //---Event handlers---

    private addSelfOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: null,
            kind: PlayerKind.User
        };

        this.props.api
            .addPlayer(this.props.game.id, request)
            .then(stateAndEvent => {
                this.props.updateGame(stateAndEvent.game);
            })
            .catch(reason => {
                alert("Add player failed because " + reason);
            });
    }

    private addGuestOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: this.state.guestName,
            kind: PlayerKind.Guest
        };

        this.props.api
            .addPlayer(this.props.game.id, request)
            .then(stateAndEvent => {
                this.setState({
                        guestName : ""
                    },
                    () => this.props.updateGame(stateAndEvent.game));
            })
            .catch(reason => {
                alert("Add player failed because " + reason);
            });
    }

    private addGuestOnChanged(event : React.ChangeEvent<HTMLInputElement>) : void {
        const name = event.target.value;
        this.setState({ guestName: name });
    }

    private removeOnClick(playerId : number) : void {
        this.props.api
            .removePlayer(this.props.game.id, playerId)
            .then(stateAndEvent => {
                const game = stateAndEvent.game;

                //Game is closed when creator is removed
                if (game.status === GameStatus.Aborted || game.status === GameStatus.AbortedWhilePending) {
                    this.props.updateGame(null);
                }
                else {
                    this.props.updateGame(game);
                }
            })
            .catch(reason => {
                alert("Remove player failed because " + reason);
            });
    }


    //---Rendering---

    private renderPlayerRow(seat : Seat, rowNumber : number) {
        switch (seat.action) {
            case SeatActionType.None:
                if (seat.player === null) {
                    return (
                        <tr key={"row" + rowNumber}>
                            <td className="lightText lobbyPlayersTableTextCell">
                                (Empty)
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    );
                } else {
                    return (
                        <tr key={"row" + rowNumber}>
                            <td className="lobbyPlayersTableTextCell">
                                {seat.player.name}
                            </td>
                            <td>{seat.note}</td>
                            <td></td>
                        </tr>
                    );
                }

            case SeatActionType.Join:
                return (
                    <tr key={"row" + rowNumber}>
                        <td className="lightText lobbyPlayersTableTextCell">
                            (Empty)
                        </td>
                        <td></td>
                        <td className="centeredContainer">
                            <ActionButton
                                label="Join"
                                onClick={() => this.addSelfOnClick()}
                            />
                        </td>
                    </tr>
                );

            case SeatActionType.AddGuest:
                return (
                    <tr key={"row" + rowNumber}>
                        <td>
                            <input
                                type="text"
                                value={this.state.guestName}
                                onChange={e => this.addGuestOnChanged(e)}
                                className="fullWidth"
                            />
                        </td>
                        <td></td>
                        <td className="centeredContainer">
                            <ActionButton
                                label="Add Guest"
                                onClick={() => this.addGuestOnClick()}
                            />
                        </td>
                    </tr>
                );

            case SeatActionType.Remove:
                const label = this.isSeatSelf(seat) ? "Quit" : "Remove";

                return (
                    <tr key={"row" + rowNumber}>
                        <td className="lobbyPlayersTableTextCell">
                            {seat.player.name}
                        </td>
                        <td>{seat.note}</td>
                        <td className="centeredContainer">
                            <ActionButton
                                label={label}
                                onClick={() => this.removeOnClick(seat.player.id)}
                            />
                        </td>
                    </tr>
                );

            default:
                return "";
        }
    }

    render() {
        if (this.props.game === null) {
            return "";
        }

        const seats = this.getSeats(this.props.game);

        return (
            <div>
                <table className="table">
                    <tbody>
                        <tr>
                            <td className="centeredContainer">
                                Seats
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table className="table">
                    <tbody>
                        {seats.map((seat, i) => this.renderPlayerRow(seat, i))}
                    </tbody>
                </table>
            </div>
        );
    }
}