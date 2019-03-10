import { User, Game } from "./api/model";

export interface PlayerAction {
    name : string,
    onClick : () => void,
    hideByDefault : boolean,
    isAvailable : boolean,
    isEnabled : boolean
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
        private readonly game : Game
    ) {
        this.actions = this.createActions();
    }

    private createActions() : PlayerAction[] {
        return [
            {
                name: "Commit",
                onClick: () => alert("Commit"),
                hideByDefault: false,
                isAvailable: true,
                isEnabled: false
            },
            {
                name: "Reset",
                onClick: () => alert("Reset"),
                hideByDefault: false,
                isAvailable: true,
                isEnabled: false
            },
            {
                name: "PlaceholderHiddenAction",
                onClick: () => alert("Placeholder"),
                hideByDefault: true,
                isAvailable: true,
                isEnabled: true
            }
        ];
    }

    getVisibleActions(showAllActions : boolean) : PlayerAction[] {
        return showAllActions
            ? this.actions
            : this.actions.filter(a => !a.hideByDefault);
    }

    getHiddenActionsState(showAllActions : boolean) : HiddenActionsState {
        const hideableActions = this.actions.filter(a => a.hideByDefault);

        if (hideableActions.length === 0) {
            return HiddenActionsState.NoneHideable;
        }

        return showAllActions
            ? HiddenActionsState.HideableShown
            : HiddenActionsState.SomeHidden;
    }
}