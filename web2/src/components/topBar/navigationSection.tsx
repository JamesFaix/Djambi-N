import * as React from 'react';
import { AppState, NavigationState } from '../../store/state';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import IconButton from '../controls/iconButton';
import Icons from '../../utilities/icons';

interface NavigationSectionProps {
    options : NavigationState
}

class navigationSection extends React.Component<NavigationSectionProps> {
    render() {
        const o = this.props.options;
        return (
            <div>
                {o.enableSignup ?
                    <IconButton
                        title={"Sign up"}
                        icon={Icons.signup}
                        onClick={() => navigateTo(Routes.signup)}
                    />
                : null}
                {o.enableLogin ?
                    <IconButton
                        title={"Log in"}
                        icon={Icons.login}
                        onClick={() => navigateTo(Routes.login)}
                    />
                : null}
                {o.enableDashboard ?
                    <IconButton
                        title={"Home"}
                        icon={Icons.home}
                        onClick={() => navigateTo(Routes.dashboard)}
                    />
                : null}
                {o.enableCreateGame ?
                    <IconButton
                        title={"Create game"}
                        icon={Icons.newGame}
                        onClick={() => navigateTo(Routes.createGame)}
                    />
                : null}
                {o.enableLobby ?
                    <IconButton
                        title={"Lobby"}
                        icon={Icons.lobby}
                        onClick={() => navigateTo(Routes.lobby(o.gameId))}
                    />
                : null}
                {o.enablePlay ?
                    <IconButton
                        title={"Play"}
                        icon={Icons.play}
                        onClick={() => navigateTo(Routes.play(o.gameId))}
                    />
                : null}
            </div>
        );
    }
};

const mapStateToProps = (state : AppState) => {
    return {
        options: state.navigation
    };
};

const NavigationSection = connect(mapStateToProps)(navigationSection);
export default NavigationSection;