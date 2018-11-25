import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';

export interface MyGamesPageProps {
    user : UserResponse,
    api : ApiClient,
}

export default class MyGamesPage extends React.Component<MyGamesPageProps> {

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"My Games"}/>
                <br/>
                <div className="navigationStrip">
                    <LinkButton label="Home" to="/dashboard"/>
                    <LinkButton label="Create Game" to="/createGame"/>
                    <LinkButton label="Find Game" to="/findGame"/>
                </div>
            </div>
        );
    }
}