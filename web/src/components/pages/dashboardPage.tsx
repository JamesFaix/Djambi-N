import * as React from 'react';
import PageTitle from '../pageTitle';
import { User } from '../../api/model';
import { Redirect } from 'react-router';
import ActionButton from '../controls/actionButton';
import LinkButton from '../controls/linkButton';
import {Kernel as K} from '../../kernel';

export interface DashboardPageProps {
    user : User,
    setUser(user: User) : void
}

export default class DashboardPage extends React.Component<DashboardPageProps> {

    private logoutOnClick() {
        K.api
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
            return <Redirect to={K.routes.home()}/>
        }

        return (
            <div>
                <PageTitle label={"Welcome, " + this.props.user.name}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <LinkButton label="My Games" to={K.routes.myGames()}/>
                    <LinkButton label="Create Game" to={K.routes.createGame()}/>
                    <LinkButton label="Find Game" to={K.routes.findGame()}/>
                    <LinkButton label="Rules" to={K.routes.rules()} newWindow={true}/>
                    <ActionButton label="Log out" onClick={() => this.logoutOnClick()}/>
                </div>
            </div>
        );
    }
}