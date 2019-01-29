import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import ApiClient from '../../api/client';
import { CreateUserRequest, User, LoginRequest } from '../../api/model';
import LabeledInput from '../controls/labeledInput';
import { Redirect } from 'react-router';
import LinkButton from '../controls/linkButton';
import ActionButton from '../controls/actionButton';
import { InputTypes } from '../../constants';
import Routes from '../../routes';

export interface SignupPageProps {
    api : ApiClient,
    user : User,
    setUser(user : User) : void
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
        const request : CreateUserRequest = {
            name: this.state.username,
            password: this.state.password
        };

        this.props.api
            .createUser(request)
            .then(_ => {
                this.setState({
                    username: "",
                    password: ""
                });

                const loginRequest : LoginRequest = {
                    username: request.name,
                    password: request.password
                };

                return this.props.api
                    .login(loginRequest);
            })
            .then(session => {
                this.props.setUser(session.user);
            })
            .catch(reason => {
                alert("User creation failed because " + reason);
            });
    }

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.user !== null) {
            return <Redirect to={Routes.dashboard()}/>
        }

        return (
            <div>
                <PageTitle label="Sign up"/>
                <br/>
                <div className="centeredContainer">
                    <LinkButton label="Home" to={Routes.home()} />
                    <LinkButton label="Login" to={Routes.login()} />
                </div>
                <br/>
                <br/>
                <div className="form">
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
                <div className="centeredContainer">
                    <ActionButton label="Submit" onClick={() => this.submitOnClick()}/>
                </div>
            </div>
        );
    }
}