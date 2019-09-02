import * as React from 'react';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import { Classes } from '../../styles/styles';
import { withRouter } from 'react-router';
import { User, Privilege, Game, GameStatus } from '../../api/model';
import { State } from '../../store/root';

interface NavigationSectionProps {
    location : any,
    user : User,
    game : Game
}

interface NavigationOptions {
    showLogin ?: boolean,
    showSignup ?: boolean,
    showHome ?: boolean,
    showCreateGame ?: boolean,
    showLobby ?: boolean,
    showPlay ?: boolean,
    showDiplomacy ?: boolean,
    showSnapshots ?: boolean,
    gameId ?: number
}

class navigationSection extends React.Component<NavigationSectionProps> {
    render() {
        const o = this.getOptionsFromProps(this.props);
        return (
            <div
                id={"navigation-section"}
                className={Classes.topBarNavigation}
            >
                {o.showSignup ?
                    <IconButton
                        icon={Icons.Pages.signup}
                        onClick={() => navigateTo(Routes.signup)}
                    />
                : null}
                {o.showLogin ?
                    <IconButton
                        icon={Icons.Pages.login}
                        onClick={() => navigateTo(Routes.login)}
                    />
                : null}
                {o.showHome ?
                    <IconButton
                        icon={Icons.Pages.home}
                        onClick={() => navigateTo(Routes.dashboard)}
                    />
                : null}
                {o.showCreateGame ?
                    <IconButton
                        icon={Icons.Pages.newGame}
                        onClick={() => navigateTo(Routes.createGame)}
                    />
                : null}
                {o.showLobby ?
                    <IconButton
                        icon={Icons.Pages.lobby}
                        onClick={() => navigateTo(Routes.lobby(o.gameId))}
                    />
                : null}
                {o.showPlay ?
                    <IconButton
                        icon={Icons.Pages.play}
                        onClick={() => navigateTo(Routes.play(o.gameId))}
                    />
                : null}
                {o.showDiplomacy ?
                    <IconButton
                        icon={Icons.Pages.diplomacy}
                        onClick={() => navigateTo(Routes.diplomacy(o.gameId))}
                    />
                : null}
                {o.showSnapshots ?
                    <IconButton
                        icon={Icons.Pages.snapshots}
                        onClick={() => navigateTo(Routes.snapshots(o.gameId))}
                    />
                : null}
            </div>
        );
    }

    private getOptionsFromProps(props : NavigationSectionProps) : NavigationOptions {
        const route : string = props.location.pathname;

        if (route === Routes.login) {
            return { showSignup: true };
        }

        if (route === Routes.signup) {
            return { showLogin: true };
        }

        if (route === Routes.dashboard) {
            return { showCreateGame: true };
        }

        if (route === Routes.createGame) {
            return { showHome: true };
        }

        if (route.startsWith("/games")) {
            const parts = route.split("/");
            //parts[0] is ""
            //parts[1] is "games"
            const gameId = parts[2];
            const page = parts[3];

            const showPlay = props.game && (props.game.status === GameStatus.InProgress);
            const showDiplomacy = props.game && (props.game.status === GameStatus.InProgress);
            const showSnapshots = props.user && (props.user.privileges.includes(Privilege.Snapshots));

            switch(page) {
                case "lobby": {
                    return {
                        showHome: true,
                        showPlay: showPlay,
                        showDiplomacy: showDiplomacy,
                        showSnapshots: showSnapshots,
                        gameId: Number(gameId)
                    };
                }
                case "play": {
                    return {
                        showHome: true,
                        showLobby: true,
                        showDiplomacy: showDiplomacy,
                        showSnapshots: showSnapshots,
                        gameId: Number(gameId)
                    };
                }
                case "diplomacy": {
                    return {
                        showHome: true,
                        showLobby: true,
                        showPlay: showPlay,
                        showSnapshots: showSnapshots,
                        gameId: Number(gameId)
                    };
                }
                case "snapshots": {
                    return {
                        showHome: true,
                        showLobby: true,
                        showPlay: showPlay,
                        showDiplomacy: showDiplomacy,
                        gameId: Number(gameId)
                    };
                }
            }
        }

        return {};
    }
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