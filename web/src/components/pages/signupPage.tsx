import * as React from 'react';
import LabeledInput from '../controls/labeledInput';
import PageTitle from '../pageTitle';
import { CreateUserRequest, LoginRequest, User } from '../../api/model';
import { InputTypes } from '../../constants';
import { Kernel as K } from '../../kernel';
import { Redirect } from 'react-router';
import { LoginPageButton, HomePageButton, SubmitButton } from '../controls/navigationButtons';

export interface SignupPageProps {
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

        K.api
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

                return K.api
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
            return <Redirect to={K.routes.dashboard()}/>
        }

        return (
            <div>
                <PageTitle label="Sign up"/>
                <br/>
                <div className={K.classes.centerAligned}>
                    <HomePageButton/>
                    <LoginPageButton/>
                </div>
                <br/>
                <br/>
                <div className={K.classes.form}>
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
                <div className={K.classes.centerAligned}>
                    <SubmitButton
                        onClick={() => this.submitOnClick()}
                    />
                </div>
            </div>
        );
    }
}