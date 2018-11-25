import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import ApiClient from '../../api/client';
import { LoginRequest, UserResponse } from '../../api/model';
import LabeledTextbox from '../labeledTextbox';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';
import ActionButton from '../actionButton';

export interface LoginPageProps {
    api : ApiClient,
    user : UserResponse,
    setUser(user : UserResponse) : void
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

    handleChange(event : React.ChangeEvent<HTMLInputElement>) {
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

    handleSubmit() {
        const request = new LoginRequest(
            this.state.username,
            this.state.password);

        this.props.api
            .login(request)
            .then(user => {
                this.setState({
                    username: "",
                    password: ""
                });
                this.props.setUser(user);
            })
            .catch(reason => {
                alert("Login failed because " + reason);
            });
    }

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to='/dashboard'/>
        }

        return (
            <div>
                <PageTitle label="Log in"/>
                <br/>
                <div className="navigationStrip">
                    <LinkButton to="/" label="Home"/>
                    <LinkButton to="/login" label="Login"/>
                </div>
                <br/>
                <br/>
                <div className="form">
                    <LabeledTextbox
                        label="Username"
                        type="text"
                        value={this.state.username}
                        handleChange={e => this.handleChange(e)}
                    />
                    <br/>
                    <LabeledTextbox
                        label="Password"
                        type="password"
                        value={this.state.password}
                        handleChange={e => this.handleChange(e)}
                    />
                    <br/>
                </div>
                <div className="formSubmitButtonBar">
                    <ActionButton label="Submit" action={() => this.handleSubmit()}/>
                </div>
            </div>
        );
    }
}