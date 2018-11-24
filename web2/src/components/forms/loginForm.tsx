import * as React from 'react';
import '../../index.css';
import LabeledTextbox from './labeledTextbox';
import { LoginRequest } from '../../api/model';
import Client from '../../api/client';

export interface LoginFormProps {

}

export interface LoginFormState {
    username : string,
    password : string
}

export default class LoginForm extends React.Component<LoginFormProps, LoginFormState> {
    constructor(props : LoginFormProps) {
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

        Client.Instance()
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
        return (
            <div>
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