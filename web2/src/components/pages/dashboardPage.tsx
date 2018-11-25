import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import { Redirect } from 'react-router';
import ActionButton from '../actionButton';

export interface DashboardPageProps {
    user : UserResponse,
    api : ApiClient,
    setUser(user: UserResponse) : void,
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

    private rulesOnClick() : void {
        const win = window.open(this.props.rulesUrl, '_blank');
        win.focus();
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
                <div className="navigationStrip">
                    <ActionButton label="Rules" onClick={() => this.rulesOnClick()}/>
                    <ActionButton label="Log out" onClick={() => this.logoutOnClick()}/>
                </div>
            </div>
        );
    }
}