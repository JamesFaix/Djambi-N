import { User, Game, Privilege, TurnStatus } from "./api/model";

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

export default class PlayerActionsService {
    private readonly actions : PlayerAction[];
    constructor(
        private readonly user : User,
        private readonly game : Game,
        private readonly commitTurn : () => void,
        private readonly resetTurn : () => void,
        private readonly navigateToSnapshotsPage : () => void
    ) {
        this.actions = this.createActions();
    }

    private hasPrivilege(p : Privilege) : boolean {
        const match = this.user.privileges.find(p2 => p2 === p);
        return match !== undefined;
    }

    private canParticipateInGame() : boolean {
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

    private createActions() : PlayerAction[] {
        return [
            {
                name: "Commit",
                onClick: () => this.commitTurn(),
                hideByDefault: false,
                isAvailable: this.canCommit()
            },
            {
                name: "Reset",
                onClick: () => this.resetTurn(),
                hideByDefault: false,
                isAvailable: this.canReset()
            },
            {
                name: "Snapshots",
                onClick: () => this.navigateToSnapshotsPage(),
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