import * as React from 'react';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import { AppState, ActiveGameState } from '../store/state';
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
        user: state.session.user
    };
};

export const ToHome = connect(mapAppStateToSessionProps)(redirectToHome);
export const ToHomeIfSession = connect(mapAppStateToSessionProps)(redirectToHomeIfSession);
export const ToHomeIfNoSession = connect(mapAppStateToSessionProps)(redirectToHomeIfNoSession);

//#endregion

//#region Game-based redirects

interface GameStateProps {
    gameState : ActiveGameState
}

const redirectToHomeIfNoGame : React.SFC<GameStateProps> = props =>
    props.gameState && props.gameState.game
        ? null
        : <Redirect to={Routes.dashboard}/>;

const mapAppStateToGameStateProps = (state : AppState) : GameStateProps => {
    return {
        gameState: state.activeGame
    };
}

export const ToHomeIfNoGame = connect(mapAppStateToGameStateProps)(redirectToHomeIfNoGame);

//#endregion