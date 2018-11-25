import * as React from 'react';
import '../../index.css';
import PageTitle from '../pageTitle';
import NavigationStrip from '../navigationStrip';
import ApiClient from '../../api/client';
import { CreateUserRequest } from '../../api/model';
import LabeledTextbox from '../labeledTextbox';

export interface SignupPageProps {
    api : ApiClient
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
            .then(createdUser => {
                this.setState({
                    username: "",
                    password: ""
                });
                alert("Successfully created user " + createdUser.id);
            });
    }

    render() {
        const links = [
            { to: '/', label: 'Home' },
            { to: '/login', label: 'Log in' },
        ];

        return (
            <div>
                <PageTitle label="Sign up"/>
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