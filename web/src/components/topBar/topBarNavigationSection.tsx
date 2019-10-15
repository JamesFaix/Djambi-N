import * as React from 'react';
import Routes from '../../routes';
import { Icons, IconInfo } from '../../utilities/icons';
import { Classes } from '../../styles/styles';
import { withRouter } from 'react-router';
import { User, Privilege, Game, GameStatus } from '../../api/model';
import Selectors from '../../selectors';
import IconButton from '../controls/iconButton';
import Controller from '../../controllers/controller';

const navigationSection : React.SFC<{ location : any }> = props => {
    const route : string = props.location.pathname;
    const game = Selectors.game();
    const user = Selectors.user();
    const o = getOptions(route, game, user);
    return (
        <div
            id={"navigation-section"}
            className={Classes.topBarNavigation}
        >
            <NavigationButton
                icon={Icons.Pages.signup}
                state={o.showSignup}
                route={Routes.signup}
            />
            <NavigationButton
                icon={Icons.Pages.login}
                state={o.showLogin}
                route={Routes.login}
            />
            <NavigationButton
                icon={Icons.Pages.home}
                state={o.showHome}
                route={Routes.dashboard}
            />
            <IconButton
                icon={Icons.Pages.rules}
                onClick={() => window.open(Routes.rules, "_blank")}
                active={o.showRules === ButtonState.Active}
            />
            <NavigationButton
                icon={Icons.Pages.settings}
                state={o.showSettings}
                route={Routes.settings}
            />
            <NavigationButton
                icon={Icons.Pages.newGame}
                state={o.showCreateGame}
                route={Routes.createGame}
            />
            <NavigationButton
                icon={Icons.Pages.search}
                state={o.showSearch}
                route={Routes.searchGames}
            />
            <NavigationButton
                icon={Icons.Pages.lobby}
                state={o.showLobby}
                route={Routes.lobby(o.gameId)}
            />
            <NavigationButton
                icon={Icons.Pages.play}
                state={o.showPlay}
                route={Routes.play(o.gameId)}
            />
            <NavigationButton
                icon={Icons.Pages.diplomacy}
                state={o.showDiplomacy}
                route={Routes.diplomacy(o.gameId)}
            />
            <NavigationButton
                icon={Icons.Pages.gameOver}
                state={o.showGameOver}
                route={Routes.gameResults(o.gameId)}
            />
            <NavigationButton
                icon={Icons.Pages.snapshots}
                state={o.showSnapshots}
                route={Routes.snapshots(o.gameId)}
            />
        </div>
    );
};

const NavigationSection = withRouter(navigationSection);
export default NavigationSection;

//#region Data structures

enum ButtonState {
    Hidden = "HIDDEN",
    Active = "ACTIVE",
    Inactive = "INACTIVE"
}

interface NavigationOptions {
    showLogin : ButtonState,
    showSignup : ButtonState,
    showHome : ButtonState,
    showRules : ButtonState,
    showCreateGame : ButtonState,
    showLobby : ButtonState,
    showPlay : ButtonState,
    showDiplomacy : ButtonState,
    showSnapshots : ButtonState,
    showSettings : ButtonState,
    showGameOver : ButtonState,
    showSearch : ButtonState,
    gameId : number
}

const defaultOptions : NavigationOptions = {
    showLogin: ButtonState.Hidden,
    showSignup: ButtonState.Hidden,
    showHome: ButtonState.Hidden,
    showRules: ButtonState.Hidden,
    showCreateGame: ButtonState.Hidden,
    showLobby: ButtonState.Hidden,
    showPlay: ButtonState.Hidden,
    showDiplomacy: ButtonState.Hidden,
    showSnapshots: ButtonState.Hidden,
    showSettings: ButtonState.Hidden,
    showGameOver: ButtonState.Hidden,
    showSearch: ButtonState.Hidden,
    gameId: null
}

enum ContextType {
    LoggedOut,
    LoggedInWithNoActiveGame,
    LoggedInWithActiveGame
}

//#endregion

//#region Get options

function getOptions(route : string, game : Game, user : User) : NavigationOptions {
    let o = { ...defaultOptions };
    let contextType = getContextType(route, game);
    //Order matters!
    applyDefaultsForContext(contextType, o);
    enableGamePageButtonsBasedOnState(game, user, o);
    markCurrentPageActive(route, o);
    getGameId(route, game, o);
    return o;
}

function isGamePage(route : string) : boolean {
    const parts = route.split("/");
    //[ "", "games", "{id}" "{page}" ]
    return parts.length === 4 && parts[1] === "games"
}

function getContextType(route : string, game : Game) : ContextType {
    if (route === Routes.login || route === Routes.signup) {
        return ContextType.LoggedOut;
    }
    else if (isGamePage(route) || game) {
        return ContextType.LoggedInWithActiveGame;
    }
    else {
        return ContextType.LoggedInWithNoActiveGame;
    }
}

function applyDefaultsForContext(type : ContextType, o : NavigationOptions) : void {
    o.showSettings = ButtonState.Inactive;
    o.showRules = ButtonState.Inactive;

    if (type === ContextType.LoggedOut) {
        o.showSignup = ButtonState.Inactive;
        o.showLogin = ButtonState.Inactive;
    } else {
        if (type === ContextType.LoggedInWithActiveGame ||
            type === ContextType.LoggedInWithNoActiveGame) {
            o.showHome = ButtonState.Inactive;
            o.showCreateGame = ButtonState.Inactive;
            o.showSearch = ButtonState.Inactive;
        }

        if (type === ContextType.LoggedInWithActiveGame) {
            o.showLobby = ButtonState.Inactive;
        }
    }
}

function enableGamePageButtonsBasedOnState(game : Game, user : User, o : NavigationOptions) : void {
    if (!game) { return; }

    switch (game.status) {
        case GameStatus.InProgress:
            o.showPlay = ButtonState.Inactive;
            o.showDiplomacy = ButtonState.Inactive;
            break;
        case GameStatus.Over:
            o.showGameOver = ButtonState.Inactive;
            break;
    }

    if (user && user.privileges.includes(Privilege.Snapshots)) {
        o.showSnapshots = ButtonState.Inactive;
    }
}

function markCurrentPageActive(route : string, o : NavigationOptions) : void {
    switch (route) {
        case Routes.login:
            o.showLogin = ButtonState.Active;
            return;
        case Routes.signup:
            o.showSignup = ButtonState.Active;
            return;
        case Routes.dashboard:
            o.showHome = ButtonState.Active;
            return;
        case Routes.createGame:
            o.showCreateGame = ButtonState.Active;
            return;
        case Routes.settings:
            o.showSettings = ButtonState.Active;
            return;
        case Routes.searchGames:
            o.showSearch = ButtonState.Active;
            return;
    }
    if (isGamePage(route)) {
        const gamePage = route.split("/")[3]; //[ "", "games", "{id}" "{page}" ]
        switch(gamePage) {
            case "lobby":
                o.showLobby = ButtonState.Active;
                break;
            case "play":
                o.showPlay = ButtonState.Active;
                break;
            case "diplomacy":
                o.showDiplomacy = ButtonState.Active;
                break;
            case "snapshots":
                o.showSnapshots = ButtonState.Active;
                break;
            case "results":
                o.showGameOver = ButtonState.Active;
                break;
        }
    }
}

function getGameId(route : string, game : Game, o : NavigationOptions) : void {
    let gameId : number = null;
    const parts = route.split("/");  //[ "", "games", "{id}" "{page}" ]
    if (parts.length === 4 // Create game page starts with 'games' but is only 3 parts
        && parts[1] === "games") {
        gameId = Number(parts[2]);
    } else if (game) {
        gameId = game.id
    }
    o.gameId = gameId;
}

//#endregion

const NavigationButton : React.SFC<{
    route : string,
    state : ButtonState,
    icon : IconInfo,
}> = props => {
    if (props.state === ButtonState.Hidden) {
        return null;
    }

    return (
        <IconButton
            icon={props.icon}
            onClick={() => Controller.navigateTo(props.route)}
            active={props.state === ButtonState.Active}
        />
    );
};