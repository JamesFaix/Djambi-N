import * as React from 'react';
import DiplomacyPlayersTable from '../tables/diplomacyPlayersTable';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../sections/basicPageContainer';
import { GameStatus, Game, User } from '../../api/model';
import ControllerEffects from '../../controllerEffects';
import { State } from '../../store/root';
import { connect } from 'react-redux';

const diplomacyPage : React.SFC<{
    user : User,
    game : Game
}> = props => {
    const i = Icons.PlayerActions;

    ControllerEffects.redirectToLoginIfNotLoggedIn(props.user);
    ControllerEffects.redirectToLobbyIfNotGameStatus(props.game, GameStatus.InProgress);

    return (
        <BasicPageContainer>
            <DiplomacyPlayersTable/>
            <br/>
            <p>
                If a player concedes (<FontAwesomeIcon icon={i.concede.icon}/>),
                they are removed from the turn cycle and all their pieces are abandoned. This cannot be undone.
                If a player concedes when it is not their turn, it does not take effect until their next turn would start.
            </p>
            <br/>
            <p>
                If all players who have not conceded or been eliminated accept a draw (<FontAwesomeIcon icon={i.acceptDraw.icon}/>),
                the game ends and no one wins. If you have accepted a draw, but not everyone has, you can revoke your acceptance at any time (<FontAwesomeIcon icon={i.revokeDraw.icon}/>).
            </p>
    </BasicPageContainer>
    );
}

const mapStateToProps = (state : State) => {
    return {
        user : state.session.user,
        game : state.activeGame.game
    };
}

const DiplomacyPage = connect(mapStateToProps)(diplomacyPage);
export default DiplomacyPage;