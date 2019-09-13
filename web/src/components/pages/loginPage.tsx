import * as React from 'react';
import RedirectToDashboardIfLoggedIn from '../utilities/redirectToDashboardIfLoggedIn';
import BasicPageContainer from '../containers/basicPageContainer';
import { SectionHeader } from '../controls/headers';
import IconButton from '../controls/iconButton';
import { Icons } from '../../utilities/icons';
import HtmlInputTypes from '../htmlInputTypes';
import Controller from '../../controllers/controller';
import Routes from '../../routes';

const LoginPage : React.SFC<{}> = _ => {
    return (
        <BasicPageContainer>
            <RedirectToDashboardIfLoggedIn/>
            <SectionHeader text="Log in"/>
            <LoginForm/>
            <br/>
            <PromptToSignupSection/>
        </BasicPageContainer>
    );
}
export default LoginPage;

const LoginForm : React.SFC<{}> = _ => {
    const [username, setUsername] = React.useState("");
    const [password, setPassword] = React.useState("");

    return (<>
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
        <br/>
        <IconButton
            icon={Icons.UserActions.login}
            showTitle={true}
            onClick={() =>  Controller.Session.login({ username: username, password: password })}
        />
    </>);
}

const PromptToSignupSection : React.SFC<{}> = _ => {
    return (
        <div style={{textAlign:"center"}}>
            <SectionHeader text="Don't have an account yet?"/>
            <br/>
            <IconButton
                icon={Icons.Pages.signup}
                showTitle={true}
                onClick={() => Controller.navigateTo(Routes.signup)}
            />
        </div>
    );
}