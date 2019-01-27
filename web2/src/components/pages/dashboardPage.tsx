import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import ActionButton from '../actionButton';
import LinkButton from '../linkButton';

export interface DashboardPageProps {
    user : User,
    api : ApiClient,
    setUser(user: User) : void,
    rulesUrl : string
}

export default class DashboardPage extends React.Component<DashboardPageProps> {

    private logoutOnClick() {
        this.props.api
            .logout()
            .then(_ => {
                this.props.setUser(null);
            })
            .catch(reason => {
                alert("Logout failed because " + reason);
            });
    }

    render() {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to='/'/>
        }

        return (
            <div>
                <PageTitle label={"Welcome, " + this.props.user.name}/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="My Games" to="/myGames"/>
                    <LinkButton label="Create Game" to="/createGame"/>
                    <LinkButton label="Find Game" to="/findGame"/>
                    <LinkButton label="Rules" to={this.props.rulesUrl} newWindow={true}/>
                    <ActionButton label="Log out" onClick={() => this.logoutOnClick()}/>
                </div>
            </div>
        );
    }
}