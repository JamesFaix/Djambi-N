import * as React from 'react';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import IconButton from '../controls/iconButton';
import { Icons, IconInfo } from '../../utilities/icons';
import { Classes } from '../../styles/styles';
import { withRouter } from 'react-router';
import { User, Privilege, Game, GameStatus } from '../../api/model';
import { State } from '../../store/root';

enum ButtonState {
    Hidden = "HIDDEN",
    Active = "ACTIVE",
    Inactive = "INACTIVE"
}

interface NavigationButtonProps {
    route : string,
    state : ButtonState,
    icon : IconInfo
}

const NavigationButton : React.SFC<NavigationButtonProps> = props => {
    if (props.state === ButtonState.Hidden) {
        return null;
    }

    return (
        <IconButton
            icon={props.icon}
            onClick={() => navigateTo(props.route)}
            active={props.state === ButtonState.Active}
        />
    );
};

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
    gameId : number
}

function getOptionsFromProps(props : NavigationSectionProps) : NavigationOptions {
    const route : string = props.location.pathname;

    const o : NavigationOptions = {
        showLogin: ButtonState.Hidden,
        showSignup: ButtonState.Hidden,
        showHome: ButtonState.Hidden,
        showCreateGame: ButtonState.Hidden,
        showLobby: ButtonState.Hidden,
        showPlay: ButtonState.Hidden,
        showDiplomacy: ButtonState.Hidden,
        showSnapshots: ButtonState.Hidden,
        gameId: null
    };

    if (route === Routes.login) {
        o.showLogin = ButtonState.Active;
        o.showSignup = ButtonState.Inactive;
    }

    else if (route === Routes.signup) {
        o.showSignup = ButtonState.Active;
        o.showLogin = ButtonState.Inactive;
    }

    else if (route === Routes.dashboard) {
        o.showHome = ButtonState.Active;
        o.showCreateGame = ButtonState.Inactive;
    }

    else if (route === Routes.createGame) {
        o.showCreateGame = ButtonState.Active;
        o.showHome = ButtonState.Inactive;
    }

    else if (route.startsWith("/games")) {
        const parts = route.split("/");
        //parts[0] is ""
        //parts[1] is "games"
        const gameId = parts[2];
        const page = parts[3];

        o.gameId = Number(gameId);
        o.showHome = ButtonState.Inactive;
        o.showLobby = ButtonState.Inactive;

        if(props.game && props.game.status === GameStatus.InProgress) {
            o.showPlay = ButtonState.Inactive;
            o.showDiplomacy = ButtonState.Inactive;
        }
        if (props.user && props.user.privileges.includes(Privilege.Snapshots)) {
            o.showSnapshots = ButtonState.Inactive;
        }

        switch(page) {
            case "lobby": {
                o.showLobby = ButtonState.Active;
                break;
            }
            case "play": {
                o.showPlay = ButtonState.Active;
                break;
            }
            case "diplomacy": {
                o.showDiplomacy = ButtonState.Active;
                break;
            }
            case "snapshots": {
                o.showSnapshots = ButtonState.Active;
                break;
            }
        }
    }

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
        game: state.activeGame.game
    };
}

let NavigationSection1 = connect(mapStateToProps)(navigationSection);
let NavigationSection = withRouter(props => <NavigationSection1 {...props}/>)
export default NavigationSection;