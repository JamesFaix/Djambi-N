import * as React from 'react';
import { CreateUserRequest } from '../../api/model';
import Routes from '../../routes';
import { Link } from 'react-router-dom';
import * as Redirects from '../redirects';
import { Dispatch } from 'redux';
import * as ThunkActions from '../../thunkActions';
import { connect } from 'react-redux';

interface SignupPageProps {
    onSignupClicked: (request: CreateUserRequest) => void
}

interface SignupPageState {
    username : string,
    password : string
}

class signupPage extends React.Component<SignupPageProps, SignupPageState>{
    constructor(props : SignupPageProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private getRequestFromForm() : CreateUserRequest {
        return {
            name: this.state.username,
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
                        onClick={() => this.props.onSignupClicked(this.getRequestFromForm())}
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

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        onSignupClicked: (request: CreateUserRequest) => ThunkActions.signup(request)(dispatch)
    };
}

const SignupPage = connect(null, mapDispatchToProps)(signupPage);

export default SignupPage;