import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';

export interface CreateLobbyPageProps {
    user : UserResponse,
    api : ApiClient,
}

export default class CreateLobbyPage extends React.Component<CreateLobbyPageProps> {

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Create Game"}/>
                <br/>
                <div className="navigationStrip">
                    <LinkButton label="Home" to="/dashboard"/>
                </div>
            </div>
        );
    }
}