import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { Redirect } from 'react-router';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import LinkButton from '../linkButton';

export interface HomePageProps {
    user : UserResponse,
    api : ApiClient,
    setUser(user : UserResponse) : void
}

export default class HomePage extends React.Component<HomePageProps> {

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to='/dashboard'/>
        }

        this.props.api
            .getCurrentUser()
            .then(user => {
                if (user !== null){
                    this.props.setUser(user);
                }
            });

        return (
            <div>
                <PageTitle label="Welcome"/>
                <br/>
                <div className="navigationStrip">
                    <LinkButton to="/signup" label="Sign up"/>
                    <LinkButton to="/login" label="Login"/>
                </div>
            </div>
        );
    }
}