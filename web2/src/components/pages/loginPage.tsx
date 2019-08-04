import * as React from 'react';
import { AppState } from '../../store/state';
import { Repository } from '../../repository';
import { LoginRequest } from '../../api/model';
import { Redirect } from 'react-router';
import Routes from '../../routes';
import { Link } from 'react-router-dom';

interface LoginPageProps {
    appState : AppState,
    repo: Repository
}

interface LoginPageState {
    username : string,
    password : string
}

export default class LoginPage extends React.Component<LoginPageProps, LoginPageState>{
    constructor(props : LoginPageProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private submitOnClick() {
        const request : LoginRequest = {
            username: this.state.username,
            password: this.state.password
        };

        this.props.repo.login(request);
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
                        Log in
                    </button>
                </div>
                <div>
                    Don't have an account yet?
                    <Link to={Routes.signup}>
                        <button>Sign up</button>
                    </Link>
                </div>
            </div>
        );
    }
}