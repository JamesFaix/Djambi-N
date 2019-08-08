import * as React from 'react';
import { connect } from 'react-redux';
import { Redirect } from 'react-router';
import { AppState } from '../store/state';
import { Session } from '../api/model';
import Routes from '../routes';

interface SessionProps {
    session : Session
}

const redirectToHome : React.SFC<SessionProps> = (props : SessionProps) =>
    props.session
        ? <Redirect to={Routes.dashboard}/>
        : <Redirect to={Routes.login}/>;

const redirectToHomeIfSession : React.SFC<SessionProps> = (props: SessionProps) =>
    props.session
        ? <Redirect to={Routes.dashboard}/>
        : null;

const redirectToHomeIfNoSession : React.SFC<SessionProps> = (props: SessionProps) =>
    props.session
        ? null
        : <Redirect to={Routes.login}/>;

const mapStateToProps = (state : AppState) : SessionProps => {
    return {
        session: state.session
    };
};

export const ToHome = connect(mapStateToProps)(redirectToHome);
export const ToHomeIfSession = connect(mapStateToProps)(redirectToHomeIfSession);
export const ToHomeIfNoSession = connect(mapStateToProps)(redirectToHomeIfNoSession);