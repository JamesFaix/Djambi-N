import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { CreateUserRequest } from '../../api/model';
import * as ThunkActions from '../../thunkActions';
import Styles from '../../styles/styles';
import SectionHeader from '../sections/sectionHeader';

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
            <div style={Styles.pageContainer()}>
                <SectionHeader text="Enter new account info"/>
                <table>
                    <tbody>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Username
                            </td>
                            <td style={Styles.noBorder()}>
                                <input
                                    type="text"
                                    value={this.state.username}
                                    onChange={e => this.setState({ username: e.target.value })}
                                >
                                </input>
                            </td>
                        </tr>
                        <tr>
                            <td style={Styles.noBorder()}>
                                Password
                            </td>
                            <td style={Styles.noBorder()}>
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
                    <button style={Styles.smallTopMargin()}
                        onClick={() => this.props.submit(this.getFormDataFromState())}
                    >
                        Create account
                    </button>
                </div>
            </div>
        );
    }
}

const mapDispatchToProps = (dispatch : Dispatch) => {
    return {
        submit: (request: CreateUserRequest) => ThunkActions.signup(request)(dispatch)
    };
};

const SignupForm = connect(null, mapDispatchToProps)(signupForm);

export default SignupForm;