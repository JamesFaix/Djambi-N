import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { User } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import ActionButton from '../controls/actionButton';
import LinkButton from '../controls/linkButton';
import Routes from '../../routes';
import StyleService from '../../styleService';

export interface DashboardPageProps {
    user : User,
    api : ApiClient,
    setUser(user: User) : void
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
            return <Redirect to={Routes.home()}/>
        }

        return (
            <div>
                <PageTitle label={"Welcome, " + this.props.user.name}/>
                <br/>
                <div className={StyleService.classCenteredContainer}>
                    <LinkButton label="My Games" to={Routes.myGames()}/>
                    <LinkButton label="Create Game" to={Routes.createGame()}/>
                    <LinkButton label="Find Game" to={Routes.findGame()}/>
                    <LinkButton label="Rules" to={Routes.rules()} newWindow={true}/>
                    <ActionButton label="Log out" onClick={() => this.logoutOnClick()}/>
                </div>
            </div>
        );
    }
}