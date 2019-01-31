import * as React from 'react';
import { User, Game, Player, PlayerKind, CreatePlayerRequest, GameStatus } from '../api/model';
import HintCell from './tables/hintCell';
import EmptyCell from './tables/emptyCell';
import TextCell from './tables/textCell';
import EmphasizedTextCell from './tables/emphasizedTextCell';
import TextFieldCell from './tables/textFieldCell';
import ActionButtonCell from './tables/actionButtonCell';

export interface GameInfoPlayersTableProps {
    user : User,
    game : Game,
    addPlayer(gameId : number, request : CreatePlayerRequest) : void,
    removePlayer(gameId : number, playerId : number) : void
}

export interface GameInfoPlayersTableState {
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

export default class GameInfoPlayersTable extends React.Component<GameInfoPlayersTableProps, GameInfoPlayersTableState> {
    constructor(props : GameInfoPlayersTableProps) {
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

                if (p.kind === PlayerKind.Neutral) {
                    seat.note = "Neutral";
                }

                if (self.isAdmin
                    || game.createdByUserId === self.id
                    || seat.player.name === self.name
                    || (seat.player.kind === PlayerKind.Guest
                        && seat.player.userId === self.id)) {

                    seat.action = SeatActionType.Remove;
                }

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

        this.props.addPlayer(this.props.game.id, request);
    }

    private addGuestOnClick() : void {
        const request : CreatePlayerRequest = {
            userId: this.props.user.id,
            name: this.state.guestName,
            kind: PlayerKind.Guest
        };

        this.props.addPlayer(this.props.game.id, request);
    }

    private addGuestOnChanged(event : React.ChangeEvent<HTMLInputElement>) : void {
        const name = event.target.value;
        this.setState({ guestName: name });
    }

    private removeOnClick(playerId : number) : void {
        this.props.removePlayer(this.props.game.id, playerId);
    }

    //---Rendering---

    private renderPlayerRow(status : GameStatus, seat : Seat, rowNumber : number) {
        switch (seat.action) {
            case SeatActionType.None:
                if (seat.player === null) {
                    return (
                        <tr key={"row" + rowNumber}>
                            <HintCell text="(Empty)" />
                            <EmptyCell/>
                            {status === GameStatus.Pending ? <EmptyCell/> : undefined}
                        </tr>
                    );
                } else {
                    return (
                        <tr key={"row" + rowNumber}>
                            <EmphasizedTextCell text={seat.player.name}/>
                            <TextCell text={seat.note} />
                            {status === GameStatus.Pending ? <EmptyCell/> : undefined}
                        </tr>
                    );
                }

            case SeatActionType.Join:
                return (
                    <tr key={"row" + rowNumber}>
                        <HintCell text="(Empty)" />
                        <EmptyCell/>
                        {status === GameStatus.Pending ?
                            <ActionButtonCell
                                label="Join"
                                onClick={() => this.addSelfOnClick()}
                            />
                            : undefined
                        }
                    </tr>
                );

            case SeatActionType.AddGuest:
                return (
                    <tr key={"row" + rowNumber}>
                        <TextFieldCell
                            value={this.state.guestName}
                            onChange={e => this.addGuestOnChanged(e)}
                        />
                        <EmptyCell/>
                        {status === GameStatus.Pending ?
                            <ActionButtonCell
                                label="Add Guest"
                                onClick={() => this.addGuestOnClick()}
                            />
                            : undefined
                        }
                    </tr>
                );

            case SeatActionType.Remove:
                return (
                    <tr key={"row" + rowNumber}>
                        <EmphasizedTextCell text={seat.player.name}/>
                        <TextCell text={seat.note} />
                        {status === GameStatus.Pending ?
                            <ActionButtonCell
                                label={this.isSeatSelf(seat) ? "Quit" : "Remove"}
                                onClick={() => this.removeOnClick(seat.player.id)}
                            />
                            : undefined
                        }
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
                        {seats.map((seat, i) => this.renderPlayerRow(this.props.game.status, seat, i))}
                    </tbody>
                </table>
            </div>
        );
    }
}