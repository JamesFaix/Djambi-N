import * as React from 'react';
import BasicPageContainer from '../sections/basicPageContainer';
import { Game, User } from '../../api/model';
import LoadGame from '../utilities/loadGame';
import { State } from '../../store/root';
import { connect } from 'react-redux';
import ControllerEffects from '../../controllerEffects';

const gameRedirectPage : React.SFC<{
    user : User,
    game : Game
}> = props => {
    const routeGameId = Number((props as any).match.params.gameId);

    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);
    ControllerEffects.redirectToGame(routeGameId, props.game);

    return (
        <BasicPageContainer>
            <LoadGame gameId={routeGameId}/>
        </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user,
        game: state.activeGame.game
    };
}

const GameRedirectPage = connect(mapStateToProps)(gameRedirectPage);
export default GameRedirectPage;
