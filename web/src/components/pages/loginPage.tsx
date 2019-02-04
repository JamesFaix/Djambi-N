import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import ApiClient from '../../api/client';
import { LoginRequest, User } from '../../api/model';
import LabeledInput from '../controls/labeledInput';
import { Redirect } from 'react-router';
import LinkButton from '../controls/linkButton';
import ActionButton from '../controls/actionButton';
import { InputTypes } from '../../constants';
import Routes from '../../routes';
import StyleService from '../../styleService';

export interface LoginPageProps {
    api : ApiClient,
    user : User,
    setUser(user : User) : void
}

export interface LoginPageState {
    username : string,
    password : string
}

export default class LoginPage extends React.Component<LoginPageProps, LoginPageState> {
    constructor(props : LoginPageProps) {
        super(props);
        this.state = {
            username: '',
            password: '',
        };
    }

    private formOnChange(event : React.ChangeEvent<HTMLInputElement>) {
        const input = event.target;
        switch (input.name) {
            case "Username":
                this.setState({ username: input.value });
                break;

            case "Password":
                this.setState({ password: input.value });
                break;

            default:
                break;
        }
    }

    private submitOnClick() {
        const request : LoginRequest = {
            username: this.state.username,
            password: this.state.password
        };

        this.props.api
            .login(request)
            .then(session => {
                this.setState({
                    username: "",
                    password: ""
                });
                this.props.setUser(session.user);
            })
            .catch(reason => {
                alert("Login failed because " + reason);
            });
    }

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to={Routes.dashboard()}/>
        }

        return (
            <div>
                <PageTitle label="Log in"/>
                <br/>
                <div className={StyleService.classCenteredContainer}>
                    <LinkButton label="Home" to={Routes.home()} />
                    <LinkButton label="Sign up" to={Routes.signup()} />
                </div>
                <br/>
                <br/>
                <div className={StyleService.classForm}>
                    <LabeledInput
                        label="Username"
                        type={InputTypes.Text}
                        value={this.state.username}
                        onChange={e => this.formOnChange(e)}
                    />
                    <br/>
                    <LabeledInput
                        label="Password"
                        type={InputTypes.Password}
                        value={this.state.password}
                        onChange={e => this.formOnChange(e)}
                    />
                    <br/>
                </div>
                <div className={StyleService.classCenteredContainer}>
                    <ActionButton label="Submit" onClick={() => this.submitOnClick()}/>
                </div>
            </div>
        );
    }
}