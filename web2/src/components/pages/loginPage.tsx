import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import ApiClient from '../../api/client';
import { LoginRequest, UserResponse } from '../../api/model';
import LabeledInput from '../labeledInput';
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
                <div className="centeredContainer">
                    <LinkButton to="/" label="Home"/>
                    <LinkButton to="/signup" label="Sign up"/>
                </div>
                <br/>
                <br/>
                <div className="form">
                    <LabeledInput
                        label="Username"
                        type="text"
                        value={this.state.username}
                        handleChange={e => this.formOnChange(e)}
                    />
                    <br/>
                    <LabeledInput
                        label="Password"
                        type="password"
                        value={this.state.password}
                        handleChange={e => this.formOnChange(e)}
                    />
                    <br/>
                </div>
                <div className="centeredContainer">
                    <ActionButton label="Submit" onClick={() => this.submitOnClick()}/>
                </div>
            </div>
        );
    }
}