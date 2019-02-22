import * as React from 'react';
import PageTitle from '../pageTitle';
import { Redirect } from 'react-router';
import { User } from '../../api/model';
import LinkButton from '../controls/linkButton';
import Routes from '../../routes';
import { Classes } from '../../styles';
import Kernel from '../../kernel';

export interface HomePageProps {
    user : User,
    setUser(user : User) : void
}

export default class HomePage extends React.Component<HomePageProps> {

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to={Routes.dashboard()}/>
        }

        Kernel.api
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
                <div className={Classes.centerAligned}>
                    <LinkButton label="Sign up" to={Routes.signup()}/>
                    <LinkButton label="Login" to={Routes.login()}/>
                    <LinkButton label="Rules" to={Routes.rules()} newWindow={true}/>
                </div>
                <br/>
                <br/>
                <div className={Classes.centerAligned}>
                    <img src={"../../../resources/djambi6.png"} height={500}/>
                </div>
            </div>
        );
    }
}