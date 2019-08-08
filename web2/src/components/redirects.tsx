import * as React from 'react';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import { AppState, GameState } from '../store/state';
import { Session, Game } from '../api/model';
import Routes from '../routes';

interface SessionProps {
    session : Session
}

interface GameStateProps {
    gameState : GameState
}

const redirectToHome : React.SFC<SessionProps> = props =>
    props.session
        ? <Redirect to={Routes.dashboard}/>
        : <Redirect to={Routes.login}/>;

const redirectToHomeIfSession : React.SFC<SessionProps> = props =>
    props.session
        ? <Redirect to={Routes.dashboard}/>
        : null;

const redirectToHomeIfNoSession : React.SFC<SessionProps> = props =>
    props.session
        ? null
        : <Redirect to={Routes.login}/>;

const redirectToHomeIfNoGame : React.SFC<GameStateProps> = props =>
    props.gameState && props.gameState.game
        ? null
        : <Redirect to={Routes.dashboard}/>;

const mapAppStateToSessionProps = (state : AppState) : SessionProps => {
    return {
        session: state.session
    };
};

const mapAppStateToGameStateProps = (state : AppState) : GameStateProps => {
    return {
        gameState: state.currentGame
    };
}

export const ToHome = connect(mapAppStateToSessionProps)(redirectToHome);
export const ToHomeIfSession = connect(mapAppStateToSessionProps)(redirectToHomeIfSession);
export const ToHomeIfNoSession = connect(mapAppStateToSessionProps)(redirectToHomeIfNoSession);
export const ToHomeIfNoGame = connect(mapAppStateToGameStateProps)(redirectToHomeIfNoGame);