import { Player, Game, User, Privilege, PlayerKind } from "../../api/model";

export enum SeatActionType {
    None,
    Remove,
    Join,
    AddGuest
}

export interface Seat {
    player : Player,
    note : string,
    action : SeatActionType
}

export function getSeats(game : Game, self : User) : Seat[] {
    if (!game) {
        return [];
    }

    //Get player seats first
    const seats = game.players
        .map(p => {
            const seat : Seat = {
                player : p,
                note : getPlayerNote(p, game),
                action : SeatActionType.None
            };

            if (self.privileges.includes(Privilege.EditPendingGames)
                || game.createdBy.userId === self.id
                || seat.player.name === self.name
                || (seat.player.kind === PlayerKind.Guest
                    && seat.player.userId === self.id)) {

                seat.action = SeatActionType.Remove;
            }

            return seat;
        });

    if (seats.length < game.parameters.regionCount) {

        //If self is not a player, add "Join" seat
        if (!isSelfAPlayer(game, self)) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.Join
            });
        //If self is a player and guests allowed, add "Add Guest" seat
        } else if (game.parameters.allowGuests) {
            seats.push({
                player : null,
                note : "",
                action : SeatActionType.AddGuest
            });
        }
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

function getPlayerNote(player : Player, game : Game) : string {
    switch (player.kind) {
        case PlayerKind.User:
            return "";

        case PlayerKind.Guest:
            const host = game.players
                .find(p => p.userId === player.userId
                    && p.kind === PlayerKind.User);

            return "Guest of " + host.name;

        case PlayerKind.Neutral:
            return "Neutral";

        default:
            throw "Invalid player kind.";
    }
}

export function isSelfAPlayer(game : Game, self : User) : boolean {
    return game.players.find(p => p.userId === self.id) !== undefined;
}

export function isSeatSelf(seat : Seat, self : User) : boolean {
    return seat.player.kind === PlayerKind.User
        && seat.player.userId === self.id;
}