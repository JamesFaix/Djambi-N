import * as React from 'react';
import Button, { ButtonKind } from './button';
import { IconKind } from '../icons/icon';
import { Kernel as K } from '../../kernel';

export interface LinkButtonProps {
    to : string,
    hint ?: string
}

export interface ActionButtonProps {
    onClick : () => void,
    hint ?: string
}

export class CreateGamePageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.New}
                to={K.routes.createGame()}
                hint="Create game"
            />
        );
    }
}

export class DashboardPageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Home}
                to={K.routes.dashboard()}
                hint="Home"
            />
        );
    }
}

export class EnterButton extends React.Component<LinkButtonProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Enter}
                to={this.props.to}
                hint={this.props.hint}
            />
        );
    }
}

export class FindGamesPageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Find}
                to={K.routes.findGame()}
                hint="Find games"
            />
        );
    }
}

export class HomePageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Home}
                to={K.routes.home()}
                hint="Home"
            />
        );
    }
}

export class LoginPageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Login}
                to={K.routes.login()}
                hint="Log in"
            />
        );
    }
}

export class MyGamesPageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.MyGames}
                to={K.routes.myGames()}
                hint="My games"
            />
        );
    }
}

export class ResetButton extends React.Component<ActionButtonProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Action}
                icon={IconKind.Reset}
                onClick={() => this.props.onClick()}
                hint={this.props.hint}
            />
        );
    }
}

export class RulesPageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Rules}
                to={K.routes.rules()}
                newWindow={true}
                hint="Rules"
            />
        );
    }
}

export class SignupPageButton extends React.Component<{}> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Signup}
                to={K.routes.signup()}
                hint="Sign up"
            />
        );
    }
}

export class SubmitButton extends React.Component<ActionButtonProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Action}
                icon={IconKind.Submit}
                onClick={() => this.props.onClick()}
                hint={this.props.hint}
            />
        );
    }
}