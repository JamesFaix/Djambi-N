import * as React from 'react';
import { LoginRequest } from '../../api/model';
import Routes from '../../routes';
import * as Redirects from '../redirects';
import * as ThunkActions from '../../thunkActions';
import { Dispatch } from 'redux';
import { connect } from 'react-redux';
import { navigateTo } from '../../history';

interface LoginPageProps {
    onLoginClicked: (request: LoginRequest) => void
}

interface LoginPageState {
    username : string,
    password : string
}

class loginPage extends React.Component<LoginPageProps, LoginPageState>{
    constructor(props : LoginPageProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private getRequestFromForm() : LoginRequest {
        return {
            username: this.state.username,
            password: this.state.password
        };
    }

    render() {
        return (
            <div>
                <Redirects.ToHomeIfSession/>
                <table>
                    <tbody>
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
                    </tbody>
                </table>
                <div>
                    <button
                        onClick={() => this.props.onLoginClicked(this.getRequestFromForm())}
                    >
                        Log in
                    </button>
                </div>
                <div>
                    Don't have an account yet?
                    <button
                        onClick={() => navigateTo(Routes.signup)}
                    >
                        Sign up
                    </button>
                </div>
            </div>
        );
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onLoginClicked: (request: LoginRequest) => ThunkActions.login(request)(dispatch)
    };
}

const LoginPage = connect(null, mapDispatchToProps)(loginPage);

export default LoginPage;