import * as React from 'react';
import '../../index.css';
import LabeledTextbox from './labeledTextbox';
import Client from '../../api/client';
import { CreateUserRequest } from '../../api/model';

export interface SignupFormProps {
}

export interface SignupFormState {
    username : string,
    password : string
}

export default class SignupForm extends React.Component<SignupFormProps, SignupFormState> {
    constructor(props : SignupFormProps) {
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

        Client.Instance()
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