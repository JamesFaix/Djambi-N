import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { CreateUserRequest } from '../../api/model';
import { Classes } from '../../styles/styles';
import { SectionHeader } from '../controls/headers';
import ApiActions from '../../apiActions';
import { VerticalSpacerSmall } from '../utilities/spacers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';

interface SignupFormProps {
    submit: (formData: CreateUserRequest) => void
}

interface SignupFormState {
    username : string,
    password : string
}

class signupForm extends React.Component<SignupFormProps, SignupFormState> {
    constructor(props : SignupFormProps) {
        super(props);
        this.state = {
            username: "",
            password: ""
        };
    }

    private getFormDataFromState() : CreateUserRequest {
        return {
            name: this.state.username,
            password: this.state.password
        };
    }

    render() {
        return (
            <div className={Classes.pageContainer}>
                <SectionHeader text="Enter new account info"/>
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
                <div>
                    <VerticalSpacerSmall/>
                    <IconButton
                        icon={Icons.UserActions.signup}
                        showTitle={true}
                        onClick={() => this.props.submit(this.getFormDataFromState())}
                    />
                </div>
            </div>
        );
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        submit: (request: CreateUserRequest) => ApiActions.signup(request)(dispatch)
    };
};

const SignupForm = connect(null, mapDispatchToProps)(signupForm);

export default SignupForm;