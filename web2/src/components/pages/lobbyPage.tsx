import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';

export interface LobbyPageProps {
    user : UserResponse,
    api : ApiClient,
    lobbyId : number
}

export default class LobbyPage extends React.Component<LobbyPageProps> {

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Lobby"}/>
                <br/>
                <div className="navigationStrip">
                    <LinkButton label="Home" to="/dashboard"/>
                </div>
            </div>
        );
    }
}