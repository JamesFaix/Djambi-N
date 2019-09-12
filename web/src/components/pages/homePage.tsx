import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import ControllerEffects from '../../controllerEffects';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import { User } from '../../api/model';

const homePage : React.SFC<{
    user : User
}> = props => {
    ControllerEffects.redirectToLoginOrDashboardPage(props.user);
    return (
        <BasicPageContainer>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user
    }
}

const HomePage = connect(mapStateToProps)(homePage);
export default HomePage;