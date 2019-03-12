import { User, Game, Privilege, TurnStatus, GameStatus, PlayerStatus } from "./api/model";

export interface PlayerAction {
    name : string,
    onClick : () => void,
    hideByDefault : boolean,
    isAvailable : boolean
}

export enum HiddenActionsState {
    NoneHideable,
    HideableShown,
    SomeHidden
}

export interface PlayerActionsController {
    commitTurn : () => void,
    resetTurn : () => void,
    openAcceptDrawModal : () => void,
    openRevokeDrawModal : () => void,
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

    private anyControllablePlayersOfStatus(status : PlayerStatus) : boolean {
        if (this.game.status !== GameStatus.Started) {
            return false;
        }

        if (this.hasPrivilege(Privilege.OpenParticipation)) {
            return this.game.players
                .filter(p => p.status === status)
                .length > 0;
        } else {
            return this.game.players
                .filter(p => p.status === status
                        && p.userId === this.user.id)
                .length > 0;
        }
    }

    private canAcceptDraw() {
        return this.anyControllablePlayersOfStatus(PlayerStatus.Alive);
    }

    private canRevokeDraw() {
        return this.anyControllablePlayersOfStatus(PlayerStatus.AcceptsDraw);
    }

    private createActions() : PlayerAction[] {
        return [
            {
                name: "Commit",
                onClick: () => this.controller.commitTurn(),
                hideByDefault: false,
                isAvailable: this.canCommit()
            },
            {
                name: "Reset",
                onClick: () => this.controller.resetTurn(),
                hideByDefault: false,
                isAvailable: this.canReset()
            },
            {
                name: "Accept Draw",
                onClick: () => this.controller.openAcceptDrawModal(),
                hideByDefault: true,
                isAvailable: this.canAcceptDraw()
            },
            {
                name: "Revoke Draw",
                onClick: () => this.controller.openRevokeDrawModal(),
                hideByDefault: true,
                isAvailable: this.canRevokeDraw()
            },
            {
                name: "Snapshots",
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