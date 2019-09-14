import * as React from 'react';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
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

    return (<>
        <SectionHeader text="Enter new account info"/>
        <table>
            <tbody>
                <tr>
                    <td>Username</td>
                    <td>
                        <input
                            type={HtmlInputTypes.Text}
                            value={username}
                            onChange={e => setUsername(e.target.value)}
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
                onClick={() => Controller.Session.signup({ name: username, password: password })}
            />
        </div>
    </>);
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