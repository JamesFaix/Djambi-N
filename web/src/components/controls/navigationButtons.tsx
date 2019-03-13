import * as React from 'react';
import Button, { ButtonKind } from './button';
import { IconKind } from '../icon';
import { Kernel as K } from '../../kernel';

export interface EmptyProps {
}

export interface LinkButtonProps {
    to : string
}

export interface ActionButtonProps {
    onClick : () => void
}

export class CreateGamePageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.New}
                to={K.routes.createGame()}
            />
        );
    }
}

export class DashboardPageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Home}
                to={K.routes.dashboard()}
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
            />
        );
    }
}

export class FindGamesPageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Find}
                to={K.routes.findGame()}
            />
        );
    }
}

export class HomePageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Home}
                to={K.routes.home()}
            />
        );
    }
}

export class LoginPageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Login}
                to={K.routes.login()}
            />
        );
    }
}

export class MyGamesPageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.MyGames}
                to={K.routes.myGames()}
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
            />
        );
    }
}

export class RulesPageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Rules}
                to={K.routes.rules()}
                newWindow={true}
            />
        );
    }
}

export class SignupPageButton extends React.Component<EmptyProps> {
    render() {
        return (
            <Button
                kind={ButtonKind.Link}
                icon={IconKind.Signup}
                to={K.routes.signup()}
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
            />
        );
    }
}