import * as React from 'react';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import IconButton from '../controls/iconButton';
import Icons from '../../utilities/icons';
import * as Navigation from '../../store/navigation';

interface NavigationSectionProps {
    options : Navigation.State
}

class navigationSection extends React.Component<NavigationSectionProps> {
    render() {
        const o = this.props.options;
        return (
            <div>
                {o.enableSignup ?
                    <IconButton
                        title={"Sign up"}
                        icon={Icons.Page.signup}
                        onClick={() => navigateTo(Routes.signup)}
                    />
                : null}
                {o.enableLogin ?
                    <IconButton
                        title={"Log in"}
                        icon={Icons.Page.login}
                        onClick={() => navigateTo(Routes.login)}
                    />
                : null}
                {o.enableDashboard ?
                    <IconButton
                        title={"Home"}
                        icon={Icons.Page.home}
                        onClick={() => navigateTo(Routes.dashboard)}
                    />
                : null}
                {o.enableCreateGame ?
                    <IconButton
                        title={"Create game"}
                        icon={Icons.Page.newGame}
                        onClick={() => navigateTo(Routes.createGame)}
                    />
                : null}
                {o.enableLobby ?
                    <IconButton
                        title={"Lobby"}
                        icon={Icons.Page.lobby}
                        onClick={() => navigateTo(Routes.lobby(o.gameId))}
                    />
                : null}
                {o.enablePlay ?
                    <IconButton
                        title={"Play"}
                        icon={Icons.Page.play}
                        onClick={() => navigateTo(Routes.play(o.gameId))}
                    />
                : null}
            </div>
        );
    }
};

const mapStateToProps = (state : State) => {
    return {
        options: state.navigation
    };
};

const NavigationSection = connect(mapStateToProps)(navigationSection);
export default NavigationSection;