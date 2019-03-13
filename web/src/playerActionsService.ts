import { User, Game, Privilege, TurnStatus, GameStatus, PlayerStatus, Player } from "./api/model";
import { IconKind } from "./components/icon";

export interface PlayerAction {
    icon : IconKind,
    onClick : () => void,
    hideByDefault : boolean,
    isAvailable : boolean,
    name : string
}

export enum HiddenActionsState {
    NoneHideable,
    HideableShown,
    SomeHidden
}

export interface PlayerActionsController {
    commitTurn : () => void,
    resetTurn : () => void,
    openStatusModal : (status : PlayerStatus) => void,
    navigateToSnapshotsPage : () => void
}

export default class PlayerActionsService {
    private readonly actions : PlayerAction[];
    constructor(
        private readonly user : User,
        private readonly game : Game,
        private readonly controller : PlayerActionsController
    ) {
        this.actions = this.createActions();
    }

    private hasPrivilege(p : Privilege) : boolean {
        const match = this.user.privileges.find(p2 => p2 === p);
        return match !== undefined;
    }

    private canParticipateInGame() : boolean {
        if (this.game.status !== GameStatus.Started) {
            return false;
        }

        if (this.hasPrivilege(Privilege.OpenParticipation)) {
            return true;
        }

        const currentPlayer = this.game.players.find(p => p.id === this.game.turnCycle[0]);

        if (currentPlayer === undefined) {
            return false;
        }

        return currentPlayer.userId === this.user.id;
    }

    private canCommit() {
        const turn = this.game.currentTurn;
        if (turn === null) {
            return false;
        }

        return turn.status === TurnStatus.AwaitingCommit
            && this.canParticipateInGame();
    }

    private canReset() {
        const turn = this.game.currentTurn;
        if (turn === null) {
            return false;
        }

        return turn.selections.length > 0
            && this.canParticipateInGame();
    }

    private getControllablePlayers() {
        if (this.hasPrivilege(Privilege.OpenParticipation)) {
            return this.game.players;
        } else {
            return this.game.players
                .filter(p => p.userId === this.user.id);
        }
    }

    public controllablePlayersThatCanChangeToStatus(status : PlayerStatus) : Player[] {
        if (this.game.status !== GameStatus.Started) {
            return [];
        }

        const players = this.getControllablePlayers();

        switch (status) {
            case PlayerStatus.Alive:
                return players.filter(p => p.status === PlayerStatus.AcceptsDraw);

            case PlayerStatus.AcceptsDraw:
                return players.filter(p => p.status === PlayerStatus.Alive);

            case PlayerStatus.Conceded:
                return players.filter(p => p.status === PlayerStatus.AcceptsDraw
                                        || p.status === PlayerStatus.Alive);

            default:
                return [];
        }
    }

    private anyControllablePlayersOfStatus(status : PlayerStatus) {
        if (this.game.status !== GameStatus.Started) {
            return false;
        }

        return this.getControllablePlayers()
            .filter(p => p.status === status)
            .length > 0;
    }

    private canChangeStatus(status : PlayerStatus) {
        if (this.game.status !== GameStatus.Started) {
            return false;
        }

        const players = this.getControllablePlayers();

        switch (status) {
            case PlayerStatus.AcceptsDraw:
                return players
                    .filter(p => p.status === PlayerStatus.Alive)
                    .length > 0;

            case PlayerStatus.Alive:
                return players
                    .filter(p => p.status === PlayerStatus.AcceptsDraw)
                    .length > 0;

            case PlayerStatus.Conceded:
                return players
                    .filter(p => p.status === PlayerStatus.AcceptsDraw
                              || p.status === PlayerStatus.Alive)
                    .length > 0;

            default:
                return false;
        }
    }

    private createActions() : PlayerAction[] {
        return [
            {
                name: "End turn",
                icon: IconKind.Submit,
                onClick: () => this.controller.commitTurn(),
                hideByDefault: false,
                isAvailable: this.canCommit()
            },
            {
                name: "Reset turn",
                icon: IconKind.Reset,
                onClick: () => this.controller.resetTurn(),
                hideByDefault: false,
                isAvailable: this.canReset()
            },
            {
                name: "Accept draw",
                icon: IconKind.AcceptDraw,
                onClick: () => this.controller.openStatusModal(PlayerStatus.AcceptsDraw),
                hideByDefault: true,
                isAvailable: this.canChangeStatus(PlayerStatus.AcceptsDraw)
            },
            {
                name: "Revoke draw",
                icon: IconKind.RevokeDraw,
                onClick: () => this.controller.openStatusModal(PlayerStatus.Alive),
                hideByDefault: true,
                isAvailable: this.canChangeStatus(PlayerStatus.Alive)
            },
            {
                name: "Concede",
                icon: IconKind.Concede,
                onClick: () => this.controller.openStatusModal(PlayerStatus.Conceded),
                hideByDefault: true,
                isAvailable: this.canChangeStatus(PlayerStatus.Conceded)
            },
            {
                name: "Manage snapshots",
                icon: IconKind.Snapshots,
                onClick: () => this.controller.navigateToSnapshotsPage(),
                hideByDefault: true,
                isAvailable: this.hasPrivilege(Privilege.Snapshots)
            }
        ];
    }

    getVisibleActions(showAllActions : boolean) : PlayerAction[] {
        return showAllActions
            ? this.actions.filter(a => a.isAvailable)
            : this.actions.filter(a => a.isAvailable && !a.hideByDefault);
    }

    getHiddenActionsState(showAllActions : boolean) : HiddenActionsState {
        const hideableActions = this.actions.filter(a => a.isAvailable && a.hideByDefault);

        if (hideableActions.length === 0) {
            return HiddenActionsState.NoneHideable;
        }

        return showAllActions
            ? HiddenActionsState.HideableShown
            : HiddenActionsState.SomeHidden;
    }
}