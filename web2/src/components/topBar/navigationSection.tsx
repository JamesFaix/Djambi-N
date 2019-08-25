import * as React from 'react';
import { AppState, NavigationState } from '../../store/state';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import { faHome, faChessBoard, faPlus, faSignInAlt, faUserPlus, faDoorOpen } from '@fortawesome/free-solid-svg-icons'
import IconButton from '../controls/iconButton';

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
                        icon={faUserPlus}
                        onClick={() => navigateTo(Routes.signup)}
                    />
                : null}
                {o.enableLogin ?
                    <IconButton
                        title={"Log in"}
                        icon={faSignInAlt}
                        onClick={() => navigateTo(Routes.login)}
                    />
                : null}
                {o.enableDashboard ?
                    <IconButton
                        title={"Home"}
                        icon={faHome}
                        onClick={() => navigateTo(Routes.dashboard)}
                    />
                : null}
                {o.enableCreateGame ?
                    <IconButton
                        title={"Create game"}
                        icon={faPlus}
                        onClick={() => navigateTo(Routes.createGame)}
                    />
                : null}
                {o.enableLobby ?
                    <IconButton
                        title={"Lobby"}
                        icon={faDoorOpen}
                        onClick={() => navigateTo(Routes.lobby(o.gameId))}
                    />
                : null}
                {o.enablePlay ?
                    <IconButton
                        title={"Play"}
                        icon={faChessBoard}
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