import * as React from 'react';
import EmphasizedTextCell from '../tableCells/emphasizedTextCell';
import EmptyCell from '../tableCells/emptyCell';
import HintCell from '../tableCells/hintCell';
import TextCell from '../tableCells/textCell';
import TextFieldCell from '../tableCells/textFieldCell';
import {
    CreatePlayerRequest,
    Game,
    GameStatus,
    Player,
    PlayerKind,
    User,
    Privilege
    } from '../../api/model';
import { Kernel as K } from '../../kernel';
import Button, { ButtonKind } from '../controls/button';
import { IconKind } from '../icons/icon';

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
                    note : K.copy.getPlayerNote(p, game),
                    action : SeatActionType.None
                };

                if (self.privileges.find(p => p === Privilege.EditPendingGames)
                    || game.createdBy.userId === self.id
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
                            <td className={K.classes.centerAligned}>
                                <Button
                                    kind={ButtonKind.Action}
                                    icon={IconKind.New}
                                    onClick={() => this.addSelfOnClick()}
                                    hint="Join"
                                />
                            </td>
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
                            <td className={K.classes.centerAligned}>
                                <Button
                                    kind={ButtonKind.Action}
                                    icon={IconKind.New}
                                    onClick={() => this.addGuestOnClick()}
                                    hint="Add guest"
                                />
                            </td>
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
                            <td className={K.classes.centerAligned}>
                                <Button
                                    kind={ButtonKind.Action}
                                    icon={IconKind.Remove}
                                    onClick={() => this.removeOnClick(seat.player.id)}
                                    hint={this.isSeatSelf(seat) ? "Quit" : "Remove"}
                                />
                            </td>
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
                <table className={K.classes.table}>
                    <tbody>
                        <tr>
                            <td className={K.classes.centerAligned}>
                                Seats
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table className={K.classes.table}>
                    <tbody>
                        {seats.map((seat, i) => this.renderPlayerRow(this.props.game.status, seat, i))}
                    </tbody>
                </table>
            </div>
        );
    }
}