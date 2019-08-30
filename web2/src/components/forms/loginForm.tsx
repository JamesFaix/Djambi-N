import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { LoginRequest } from '../../api/model';
import { Classes } from '../../styles/styles';
import { SectionHeader } from '../controls/headers';
import ApiActions from '../../apiActions';
import { VerticalSpacerSmall } from '../utilities/spacers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';

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
        return (
            <div className={Classes.pageContainer}>
                <SectionHeader text="Log in"/>
                <table>
                    <tbody>
                        <tr>
                            <td>Username</td>
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
                            <td>Password</td>
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
                <VerticalSpacerSmall/>
                <IconButton
                    icon={Icons.UserActions.login}
                    showTitle={true}
                    onClick={() => this.props.submit(this.getFormDataFromState())}
                />
            </div>
        );
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        submit: (formData : LoginRequest) => ApiActions.login(formData)(dispatch)
    };
};

const LoginForm = connect(null, mapDispatchToProps)(loginForm);

export default LoginForm;