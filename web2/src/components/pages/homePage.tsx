import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { Redirect } from 'react-router';
import { User } from '../../api/model';
import ApiClient from '../../api/client';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';

export interface HomePageProps {
    user : User,
    api : ApiClient,
    setUser(user : User) : void,
    rulesUrl : string
}

export default class HomePage extends React.Component<HomePageProps> {

    private rulesOnClick() : void {
        const win = window.open(this.props.rulesUrl, '_blank');
        win.focus();
    }

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
                <PageTitle label="Greetings, visitor"/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton to="/signup" label="Sign up"/>
                    <LinkButton to="/login" label="Login"/>
                    <ActionButton label="Rules" onClick={() => this.rulesOnClick()}/>
                </div>
                <br/>
                <br/>
                <div className="centeredContainer">
                    <img src={"../../../resources/djambi6.png"} height={500}/>
                </div>
            </div>
        );
    }
}