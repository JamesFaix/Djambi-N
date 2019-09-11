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
        showSettings: ButtonState.Hidden,
        showGameOver: ButtonState.Hidden,
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
        o.showSettings = ButtonState.Inactive;
        o.showCreateGame = ButtonState.Inactive;
    }

    else if (route === Routes.createGame) {
        o.showCreateGame = ButtonState.Active;
        o.showSettings = ButtonState.Inactive;
        o.showHome = ButtonState.Inactive;
    }

    else if (route === Routes.settings) {
        o.showSettings = ButtonState.Active;
        o.showHome = ButtonState.Inactive;
        o.showCreateGame = ButtonState.Inactive;
    }

    let gameId : number = null;
    let gamePage : string = null;

    if (route.startsWith("/games")) {
        const parts = route.split("/");
        //parts[0] is ""
        //parts[1] is "games"
        gameId = Number(parts[2]);
        gamePage = parts[3];
    } else if (props.game) {
        gameId = props.game.id
    }

    if (gameId) {
        o.gameId = gameId;
        o.showHome = ButtonState.Inactive;
        o.showSettings = ButtonState.Inactive;
        o.showLobby = ButtonState.Inactive;

        if(props.game) {
            switch (props.game.status) {
                case GameStatus.InProgress:
                    o.showPlay = ButtonState.Inactive;
                    o.showDiplomacy = ButtonState.Inactive;
                    break;
                case GameStatus.Over:
                    o.showGameOver = ButtonState.Inactive;
                    break;
            }
        }
        if (props.user && props.user.privileges.includes(Privilege.Snapshots)) {
            o.showSnapshots = ButtonState.Inactive;
        }

        switch(gamePage) {
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
            case "results": {
                o.showGameOver = ButtonState.Active;
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