import * as React from 'react';
import { AppState, NavigationState } from '../../store/state';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { navigateTo } from '../../history';
import Routes from '../../routes';

interface NavigationSectionProps {
    options : NavigationState,
    redirect: (route: string) => void
}

class navigationSection extends React.Component<NavigationSectionProps> {
    render() {
        const o = this.props.options;
        return (
            <div>
                {this.renderButtonIf(o.enableSignup, "Sign up", Routes.signup)}
                {this.renderButtonIf(o.enableLogin, "Login", Routes.login)}
                {this.renderButtonIf(o.enableDashboard, "Home", Routes.dashboard)}
                {this.renderButtonIf(o.enableCreateGame, "Create Game", Routes.createGame)}
                {this.renderButtonIf(o.enableLobby, "Lobby", Routes.lobby(o.gameId))}
                {this.renderButtonIf(o.enablePlay, "Play", Routes.play(o.gameId))}
            </div>
        );
    }

    private renderButtonIf(
        condition: boolean,
        label: string,
        route: string) : JSX.Element {
        if (!condition) {
            return null;
        }

        return(
            <button
                onClick={() => this.props.redirect(route)}
            >
                {label}
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