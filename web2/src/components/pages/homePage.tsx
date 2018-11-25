import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import { Redirect } from 'react-router';
import { UserResponse } from '../../api/model';
import ApiClient from '../../api/client';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';

export interface HomePageProps {
    user : UserResponse,
    api : ApiClient,
    setUser(user : UserResponse) : void,
    rulesUrl : string
}

export default class HomePage extends React.Component<HomePageProps> {

    private rulesButtonClick() : void {
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
                <div className="navigationStrip">
                    <LinkButton to="/signup" label="Sign up"/>
                    <LinkButton to="/login" label="Login"/>
                    <ActionButton label="Rules" action={() => this.rulesButtonClick()}/>
                </div>
                <br/>
                <br/>
                <div className="navigationStrip">
                    <img src={"../../../resources/djambi6.png"} height={500}/>
                </div>
            </div>
        );
    }
}