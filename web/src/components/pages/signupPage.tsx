import * as React from 'react';
import { SectionHeader } from '../controls/headers';
import IconButton, { IconSubmitButton } from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';
import BasicPageContainer from '../containers/basicPageContainer';
import Routes from '../../routes';

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
    const [password, setPassword] = React.useState("");

    return (
        <form
            style={{
                display: "flex",
                flexDirection: "column"
            }}
            onSubmit={() => Controller.Session.signup({ name: username, password: password })}
        >
            <table>
                <tbody>
                    <tr>
                        <td>Username</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Text}
                                value={username}
                                onChange={e => setUsername(e.target.value)}
                                autoComplete="username"
                                autoFocus
                            >
                            </input>
                        </td>
                    </tr>
                    <tr>
                        <td>Password</td>
                        <td>
                            <input
                                type={HtmlInputTypes.Password}
                                value={password}
                                onChange={e => setPassword(e.target.value)}
                                autoComplete="new-password"
                            >
                            </input>
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