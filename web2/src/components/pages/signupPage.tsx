import * as React from 'react';
import { AppState } from '../../store/state';
import { Repository } from '../../repository';
import { CreateUserRequest } from '../../api/model';
import { Redirect } from 'react-router';
import Routes from '../../routes';
import { Link } from 'react-router-dom';

interface SignupPageProps {
    appState : AppState,
    repo: Repository
}

interface SignupPageState {
    username : string,
    password : string
}

export default class SignupPage extends React.Component<SignupPageProps, SignupPageState>{
    constructor(props : SignupPageProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private submitOnClick() {
        const request : CreateUserRequest = {
            name: this.state.username,
            password: this.state.password
        };

        this.props.repo.signup(request);
    }

    render() {
        //Go straight to dashboard if already logged in
        if (this.props.appState.session) {
            return <Redirect to={Routes.dashboard}/>;
        }

        return (
            <div>
                <table>
                    <tr>
                        <td>Username:</td>
                        <td>
                            <input
                                type="text"
                                value={this.state.username}
                                onChange={e => this.setState({ username: e.target.value })}
                            >
                            </input>
                        </td>
                    </tr>
                    <tr>
                        <td>Password:</td>
                        <td>
                            <input
                                type="password"
                                value={this.state.password}
                                onChange={e => this.setState({ password: e.target.value })}
                            >
                            </input>
                        </td>
                    </tr>
                </table>
                <div>
                    <button
                        onClick={() => this.submitOnClick()}
                    >
                        Sign up
                    </button>
                </div>
                <div>
                    Already have an account?
                    <Link to={Routes.login}>
                        <button>Log in</button>
                    </Link>
                </div>
            </div>
        );
    }
}