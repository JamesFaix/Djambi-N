import * as React from 'react';
import PageTitle from '../pageTitle';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';
import { User } from '../../api/model';
import { RulesPageButton, SignupPageButton, LoginPageButton } from '../controls/navigationButtons';

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
                    <SignupPageButton/>
                    <LoginPageButton/>
                    <RulesPageButton/>
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