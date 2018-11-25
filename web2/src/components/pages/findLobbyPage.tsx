import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';

export interface FindLobbyPageProps {
    user : UserResponse,
    api : ApiClient,
}

export default class FindLobbyPage extends React.Component<FindLobbyPageProps> {

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Find Game"}/>
                <br/>
                <div className="navigationStrip">
                    <LinkButton label="Home" to="/dashboard"/>
                </div>
            </div>
        );
    }
}