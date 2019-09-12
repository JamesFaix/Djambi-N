import * as React from 'react';
import SignupForm from '../forms/signupForm';
import PromptToLoginSection from '../sections/promptToLoginSection';
import BasicPageContainer from '../sections/basicPageContainer';
import ControllerEffects from '../../controllerEffects';
import { User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';

const signupPage : React.SFC<{
    user : User
}> = props => {
    ControllerEffects.redirectToDashboardIfLoggedIn(props.user);

    return (
        <BasicPageContainer>
            <SignupForm/>
            <br/>
            <PromptToLoginSection/>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user
    };
}

const SignupPage = connect(mapStateToProps)(signupPage);
export default SignupPage;