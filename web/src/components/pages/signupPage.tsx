import * as React from 'react';
import { SectionHeader } from '../controls/headers';
import IconButton, { IconSubmitButton } from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import Controller from '../../controllers/controller';
import BasicPageContainer from '../containers/basicPageContainer';
import Routes from '../../routes';
import { TextInput, PasswordInput } from '../controls/input';
import { NotificationType } from '../../store/notifications';

const SignupPage : React.SFC<{}> = _ => {
    React.useEffect(() => {
        Controller.Session.redirectToDashboardIfLoggedIn();
    })
    return (
        <BasicPageContainer>
            <SectionHeader text="Enter new account info"/>
            <SignupForm/>
            <br/>
            <PromptToLoginSection/>
        </BasicPageContainer>
    );
};
export default SignupPage;

const SignupForm : React.SFC<{}> = _ => {
    const [username, setUsername] = React.useState("");
    const [password1, setPassword1] = React.useState("");
    const [password2, setPassword2] = React.useState("");

    return (
        <form
            onSubmit={e => {
                e.preventDefault();
                if (password1 !== password2) {
                    Controller.addNotification(NotificationType.Error, "Password fields do not match.");
                }
                else {
                    Controller.Session.signup({ name: username, password: password1 });
                }
            }}
        >
            <table>
                <tbody>
                    <tr>
                        <td>Username</td>
                        <td>
                            <TextInput
                                value={username}
                                onChange={x => setUsername(x)}
                                autoComplete="username"
                                autoFocus
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Password</td>
                        <td>
                            <PasswordInput
                                value={password1}
                                onChange={x => setPassword1(x)}
                                autoComplete="new-password"
                            />
                        </td>
                    </tr>
                    <tr>
                        <td>Reenter password</td>
                        <td>
                            <PasswordInput
                                value={password2}
                                onChange={x => setPassword2(x)}
                                autoComplete="new-password"
                            />
                        </td>
                    </tr>
                </tbody>
            </table>
            <br/>
            <IconSubmitButton
                icon={Icons.UserActions.signup}
                showTitle={true}
            />
        </form>
    );
}

const PromptToLoginSection : React.SFC<{}> = props => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Already have an account?"/>
            <br/>
            <IconButton
                icon={Icons.Pages.login}
                showTitle={true}
                onClick={() => Controller.navigateTo(Routes.login)}
            />
        </div>
    );
}