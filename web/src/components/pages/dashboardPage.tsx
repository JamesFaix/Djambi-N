import * as React from 'react';
import PageTitle from '../pageTitle';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';
import { User } from '../../api/model';
import { MyGamesPageButton, FindGamesPageButton, CreateGamePageButton, RulesPageButton } from '../controls/navigationButtons';
import Button, { ButtonKind } from '../controls/button';
import { IconKind } from '../icons/icon';

export interface DashboardPageProps {
    user : User,
    setUser(user : User) : void
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

    public render() : JSX.Element {
        //Go to home if not logged in
        if (this.props.user === null) {
            return <Redirect to={K.routes.home()}/>;
        }

        return (
            <div>
                <PageTitle label={"Welcome, " + this.props.user.name}/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <MyGamesPageButton/>
                    <CreateGamePageButton/>
                    <FindGamesPageButton/>
                    <RulesPageButton/>
                    <Button
                        kind={ButtonKind.Action}
                        icon={IconKind.Logout}
                        onClick={() => this.logoutOnClick()}
                        hint="Log out"
                    />
                </div>
            </div>
        );
    }
}