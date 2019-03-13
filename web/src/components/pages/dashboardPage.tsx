import * as React from 'react';
import PageTitle from '../pageTitle';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';
import { User } from '../../api/model';
import Icon, { IconKind } from '../icon';
import Button, { ButtonKind } from '../controls/button';

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
                    <Button
                        kind={ButtonKind.Link}
                        icon={IconKind.MyGames}
                        to={K.routes.myGames()}
                    />
                    <Button
                        kind={ButtonKind.Link}
                        icon={IconKind.New}
                        to={K.routes.createGame()}
                    />
                    <Button
                        kind={ButtonKind.Link}
                        icon={IconKind.Find}
                        to={K.routes.findGame()}
                    />
                    <Button
                        kind={ButtonKind.Link}
                        icon={IconKind.Rules}
                        to={K.routes.rules()}
                        newWindow={true}
                    />
                    <Button
                        kind={ButtonKind.Action}
                        icon={IconKind.Logout}
                       onClick={() => this.logoutOnClick()}
                    />
                </div>
            </div>
        );
    }
}