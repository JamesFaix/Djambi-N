import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';
import ApiClient from '../../api/client';
import { LoginRequest } from '../../api/model';
import LabeledTextbox from '../labeledTextbox';

export interface LoginPageProps {
    api : ApiClient
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
                alert("Successfully logged in as " + user.name);
            });
    }

    render() {
        const links = [
            { to: '/', label: 'Home' },
            { to: '/signup', label: 'Sign up' },
        ];

        return (
            <div>
                <PageTitle label="Log in"/>
                <br/>
                <NavigationStrip links={links}/>
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