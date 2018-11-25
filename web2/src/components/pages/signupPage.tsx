import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import ApiClient from '../../api/client';
import { CreateUserRequest, UserResponse, LoginRequest } from '../../api/model';
import LabeledTextbox from '../labeledTextbox';
import { Redirect } from 'react-router';
import LinkButton from '../linkButton';

export interface SignupPageProps {
    api : ApiClient,
    user : UserResponse,
    setUser(user : UserResponse) : void
}

export interface SignupPageState {
    username : string,
    password : string
}

export default class SignupPage extends React.Component<SignupPageProps, SignupPageState> {
    constructor(props : SignupPageProps) {
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
        const request = new CreateUserRequest(
            this.state.username,
            this.state.password);

        this.props.api
            .createUser(request)
            .then(_ => {
                this.setState({
                    username: "",
                    password: ""
                });

                const loginRequest = new LoginRequest(request.name, request.password);

                return this.props.api
                    .login(loginRequest);
            })
            .then(user => {
                this.props.setUser(user);
            })
            .catch(reason => {
                alert("User creation failed because " + reason);
            });
    }

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to='/dashboard'/>
        }

        return (
            <div>
                <PageTitle label="Sign up"/>
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
                    <button onClick={_ => this.handleSubmit()}>
                        Submit
                    </button>
                </div>
            </div>
        );
    }
}