import * as React from 'react';
import { AppState, NavigationState } from '../../store/state';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faHome, faChessBoard, faPlus, faSignInAlt, faUserPlus, faDoorOpen } from '@fortawesome/free-solid-svg-icons'
import Styles from '../../styles/styles';

interface NavigationSectionProps {
    options : NavigationState,
    redirect: (route: string) => void
}

class navigationSection extends React.Component<NavigationSectionProps> {
    render() {
        const o = this.props.options;
        return (
            <div>
                {this.renderButtonIf(o.enableSignup, <FontAwesomeIcon icon={faUserPlus}/>, "Sign up", Routes.signup)}
                {this.renderButtonIf(o.enableLogin, <FontAwesomeIcon icon={faSignInAlt}/>, "Log in", Routes.login)}
                {this.renderButtonIf(o.enableDashboard, <FontAwesomeIcon icon={faHome}/>, "Home", Routes.dashboard)}
                {this.renderButtonIf(o.enableCreateGame, <FontAwesomeIcon icon={faPlus}/>, "Create game", Routes.createGame)}
                {this.renderButtonIf(o.enableLobby, <FontAwesomeIcon icon={faDoorOpen}/>, "Lobby", Routes.lobby(o.gameId))}
                {this.renderButtonIf(o.enablePlay, <FontAwesomeIcon icon={faChessBoard}/>, "Play", Routes.play(o.gameId))}
            </div>
        );
    }

    private renderButtonIf(
        condition: boolean,
        contents: any,
        title: string,
        route: string) : JSX.Element {
        if (!condition) {
            return null;
        }
        return(
            <button
                onClick={() => this.props.redirect(route)}
                title={title}
                style={Styles.iconButton()}
            >
                {contents}
            </button>
        );
    }
};

const mapStateToProps = (state : AppState) => {
    return {
        options: state.navigation
    };
};

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        redirect: (route : string) => navigateTo(route)
    };
}

const NavigationSection = connect(mapStateToProps, mapDispatchToProps)(navigationSection);

export default NavigationSection;