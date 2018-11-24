import React from 'react';
import '../../index.css';
import LabeledTextbox from './labeledTextbox';

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

    handleSubmit(event : React.MouseEvent<HTMLButtonElement>) {
        alert('username: ' + this.state.username + ' | password: ' + this.state.password);
        event.preventDefault();
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
                    <button onClick={e => this.handleSubmit(e)}>
                        Submit
                    </button>
                </div>
            </div>
        );
    }
}