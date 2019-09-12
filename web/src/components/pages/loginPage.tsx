import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import ControllerEffects from '../../controllerEffects';
import { User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import PromptToSignupSection from '../sections/promptToSignupSection';
import LoginForm from '../forms/loginForm';

const loginPage : React.SFC<{
    user : User
}> = props => {
    ControllerEffects.redirectToDashboardIfLoggedIn(props.user);

    return (
        <BasicPageContainer>
            <LoginForm/>
            <br/>
            <PromptToSignupSection/>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user
    };
}

const LoginPage = connect(mapStateToProps)(loginPage);
export default LoginPage;