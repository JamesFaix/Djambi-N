import * as React from 'react';
import PageTitle from '../pageTitle';
import { Redirect } from 'react-router';
import { User } from '../../api/model';
import LinkButton from '../controls/linkButton';
import {Kernel as K} from '../../kernel';

export interface HomePageProps {
    user : User,
    setUser(user : User) : void
}

export default class HomePage extends React.Component<HomePageProps> {

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to={K.routes.dashboard()}/>
        }

        K.api
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
                <div className={K.classes.centerAligned}>
                    <LinkButton label="Sign up" to={K.routes.signup()}/>
                    <LinkButton label="Login" to={K.routes.login()}/>
                    <LinkButton label="Rules" to={K.routes.rules()} newWindow={true}/>
                </div>
                <br/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <img src={"../../../resources/djambi6.png"} height={500}/>
                </div>
            </div>
        );
    }
}