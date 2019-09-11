import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { LoginRequest } from '../../api/model';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controller';

interface LoginFormProps {
    submit: (formData: LoginRequest) => void
}

interface LoginFormState {
    username : string,
    password : string
}

class loginForm extends React.Component<LoginFormProps, LoginFormState> {
    constructor(props : LoginFormProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private getFormDataFromState() : LoginRequest {
        return {
            username: this.state.username,
            password: this.state.password
        };
    }

    render() {
        return (<>
            <SectionHeader text="Log in"/>
            <table>
                <tbody>
                    <tr>
                        <td>Username</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={this.state.username}
                                onChange={e => this.setState({ username: e.target.value })}
                            >
                            </input>
                        </td>
                    </tr>
                    <tr>
                        <td>Password</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Password}
                                value={this.state.password}
                                onChange={e => this.setState({ password: e.target.value })}
                            >
                            </input>
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconButton
                icon={Icons.UserActions.login}
                showTitle={true}
                onClick={() => this.props.submit(this.getFormDataFromState())}
            />
        </>);
    }
}

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        submit: (formData : LoginRequest) => Controller.Session.login(formData)
    };
};

const LoginForm = connect(null, mapDispatchToProps)(loginForm);

export default LoginForm;