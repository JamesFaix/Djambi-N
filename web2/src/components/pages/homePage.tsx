import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';
import { Redirect } from 'react-router';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';

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

        const links = [
            { to: '/signup', label: 'Sign up' },
            { to: '/login', label: 'Login' },
        ];

        return (
            <div>
                <PageTitle label="Welcome"/>
                <br/>
                <NavigationStrip links={links}/>
            </div>
        );
    }
}