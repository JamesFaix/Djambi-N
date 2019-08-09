import * as React from 'react';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import { AppState, GameState } from '../store/state';
import { User } from '../api/model';
import Routes from '../routes';
import { Dispatch } from 'redux';
import * as Actions from '../store/actions';

//#region User-based redirects

interface UserProps {
    user : User
}

const redirectToHome : React.SFC<UserProps> = props =>
    props.user
        ? <Redirect to={Routes.dashboard}/>
        : <Redirect to={Routes.login}/>;

const redirectToHomeIfSession : React.SFC<UserProps> = props =>
    props.user
        ? <Redirect to={Routes.dashboard}/>
        : null;

const redirectToHomeIfNoSession : React.SFC<UserProps> = props =>
    props.user
        ? null
        : <Redirect to={Routes.login}/>;

const mapAppStateToSessionProps = (state : AppState) : UserProps => {
    return {
        user: state.user
    };
};

export const ToHome = connect(mapAppStateToSessionProps)(redirectToHome);
export const ToHomeIfSession = connect(mapAppStateToSessionProps)(redirectToHomeIfSession);
export const ToHomeIfNoSession = connect(mapAppStateToSessionProps)(redirectToHomeIfNoSession);

//#endregion

//#region Game-based redirects

interface GameStateProps {
    gameState : GameState
}

const redirectToHomeIfNoGame : React.SFC<GameStateProps> = props =>
    props.gameState && props.gameState.game
        ? null
        : <Redirect to={Routes.dashboard}/>;

const mapAppStateToGameStateProps = (state : AppState) : GameStateProps => {
    return {
        gameState: state.currentGame
    };
}

export const ToHomeIfNoGame = connect(mapAppStateToGameStateProps)(redirectToHomeIfNoGame);

//#endregion

//#region Action-triggered redirects

interface RouteProps {
    route : string,
    onRedirectSuccesful: () => void
}

const redirectIfPendingRedirectAction : React.SFC<RouteProps> = props =>
{
    console.log("Redirects.CompleteRedirectAction");
    if (!props.route) {
        return null;
    }

    const currentUrl = window.location.href;
    const routeStartIndex = currentUrl.indexOf('#') + 1;
    const currentRoute = currentUrl.slice(routeStartIndex);

    if (props.route === currentRoute) {
        console.log("successful");
        props.onRedirectSuccesful();
        return null;
    } else {
        return <Redirect to={props.route}/>;
    }
};

const mapAppStateToRouteProps = (state : AppState) => {
    return {
        route: state.redirectRoute
    };
}

const mapDispatchToRouteProps = (dispatch : Dispatch) => {
    return {
        onRedirectSuccesful: () => dispatch(Actions.redirectSuccess())
    };
}

export const CompleteRedirectAction = connect(mapAppStateToRouteProps, mapDispatchToRouteProps)(redirectIfPendingRedirectAction);

//#endregion