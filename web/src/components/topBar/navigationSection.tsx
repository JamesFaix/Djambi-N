import * as React from 'react';
import { connect } from 'react-redux';
import Routes from '../../routes';
import { Icons } from '../../utilities/icons';
import { Classes } from '../../styles/styles';
import { withRouter } from 'react-router';
import { User, Privilege, Game, GameStatus } from '../../api/model';
import { State } from '../../store/root';
import NavigationButton, { ButtonState } from './navigationButton';

interface NavigationSectionProps {
    location : any,
    user : User,
    game : Game
}

interface NavigationOptions {
    showLogin : ButtonState,
    showSignup : ButtonState,
    showHome : ButtonState,
    showCreateGame : ButtonState,
    showLobby : ButtonState,
    showPlay : ButtonState,
    showDiplomacy : ButtonState,
    showSnapshots : ButtonState,
    showSettings : ButtonState,
    showGameOver : ButtonState,
    gameId : number
}

const defaultOptions : NavigationOptions = {
    showLogin: ButtonState.Hidden,
    showSignup: ButtonState.Hidden,
    showHome: ButtonState.Hidden,
    showCreateGame: ButtonState.Hidden,
    showLobby: ButtonState.Hidden,
    showPlay: ButtonState.Hidden,
    showDiplomacy: ButtonState.Hidden,
    showSnapshots: ButtonState.Hidden,
    showSettings: ButtonState.Hidden,
    showGameOver: ButtonState.Hidden,
    gameId: null
}

enum ContextType {
    LoggedOut,
    LoggedInWithNoActiveGame,
    LoggedInWithActiveGame
}

function getContextType(props : NavigationSectionProps) : ContextType {
    const loc : string = props.location.pathname;
    if (loc === "login" || loc === "signup") {
        return ContextType.LoggedOut;
    }
    else if (loc.startsWith("/games") || props.game) {
        return ContextType.LoggedInWithActiveGame;
    }
    else {
        return ContextType.LoggedInWithNoActiveGame;
    }
}

function applyDefaultsForContext(props : NavigationSectionProps, o : NavigationOptions) : void {
    const type = getContextType(props);
    if (type === ContextType.LoggedOut) {
        o.showSignup = ButtonState.Inactive;
        o.showLogin = ButtonState.Inactive;
    } else {
        if (type === ContextType.LoggedInWithActiveGame ||
            type === ContextType.LoggedInWithNoActiveGame) {
            o.showHome = ButtonState.Inactive;
            o.showCreateGame = ButtonState.Inactive;
            o.showSettings = ButtonState.Inactive;
        }

        if (type === ContextType.LoggedInWithActiveGame) {
            o.showLobby = ButtonState.Inactive;
        }
    }
}

function markCurrentPageActive(props : NavigationSectionProps, o : NavigationOptions) : void {
    const loc : string = props.location.pathname;
    if (loc.startsWith("/games")) {
        const parts = loc.split("/");
        const gamePage = parts[3]; //[ "", "games", "{id}" "{page}" ]
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
    } else {
        switch (loc) {
            case Routes.login:
                o.showLogin = ButtonState.Active;
                break;
            case(Routes.signup):
                o.showSignup = ButtonState.Active;
                break;
            case(Routes.dashboard) :
                o.showHome = ButtonState.Active;
                break;
            case(Routes.createGame) :
                o.showCreateGame = ButtonState.Active;
                break;
            case(Routes.settings) :
                o.showSettings = ButtonState.Active;
                break;
        }
    }
}

function enableGamePageButtonsBasedOnState(props : NavigationSectionProps, o : NavigationOptions) : void {
    if (!props.game) { return; }

    switch (props.game.status) {
        case GameStatus.InProgress:
            o.showPlay = ButtonState.Inactive;
            o.showDiplomacy = ButtonState.Inactive;
            break;
        case GameStatus.Over:
            o.showGameOver = ButtonState.Inactive;
            break;
    }

    if (props.user && props.user.privileges.includes(Privilege.Snapshots)) {
        o.showSnapshots = ButtonState.Inactive;
    }
}

function getGameId(props : NavigationSectionProps, o : NavigationOptions) : void {
    const loc : string = props.location.pathname;
    let gameId : number = null;
    if (loc.startsWith("/games")) {
        const parts = loc.split("/");
        gameId = Number(parts[2]); //[ "", "games", "{id}" "{page}" ]
    } else if (props.game) {
        gameId = props.game.id
    }
    o.gameId = gameId;
}

function getOptionsFromProps(props : NavigationSectionProps) : NavigationOptions {
    let o = { ...defaultOptions };
    applyDefaultsForContext(props, o);
    enableGamePageButtonsBasedOnState(props, o);
    markCurrentPageActive(props, o);
    getGameId(props, o);
    return o;
}

const navigationSection : React.SFC<NavigationSectionProps> = props => {
    const o = getOptionsFromProps(props);
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
                route={Routes.gameOver(o.gameId)}
            />
            <NavigationButton
                icon={Icons.Pages.snapshots}
                state={o.showSnapshots}
                route={Routes.snapshots(o.gameId)}
            />
        </div>
    );
};

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game,
    };
}

let NavigationSection1 = connect(mapStateToProps)(navigationSection);
let NavigationSection = withRouter(props => <NavigationSection1 {...props}/>)
export default NavigationSection;