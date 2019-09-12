import * as React from 'react';
import CreateGameForm from '../forms/createGameForm';
import BasicPageContainer from '../sections/basicPageContainer';
import LoadAllBoards from '../utilities/loadAllBoards';
import { User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import ControllerEffects from '../../controllerEffects';

const createGamePage : React.SFC<{
    user : User
}> = props => {
    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);

    return (
        <BasicPageContainer>
            <LoadAllBoards/>
            <CreateGameForm/>
        </BasicPageContainer>
    );
};

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user
    };
}

const CreateGamePage = connect(mapStateToProps)(createGamePage);
export default CreateGamePage;