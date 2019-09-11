import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { CreateUserRequest } from '../../api/model';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controller';

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
        return (<>
            <SectionHeader text="Enter new account info"/>
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
            <div>
                <br/>
                <IconButton
                    icon={Icons.UserActions.signup}
                    showTitle={true}
                    onClick={() => this.props.submit(this.getFormDataFromState())}
                />
            </div>
        </>);
    }
}

const mapDispatchToProps = (_ : Dispatch) => {
    return {
        submit: (request: CreateUserRequest) => Controller.Session.signup(request)
    };
};

const SignupForm = connect(null, mapDispatchToProps)(signupForm);

export default SignupForm;