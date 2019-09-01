import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import DiplomacyPlayersTable from '../tables/diplomacyPlayersTable';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Icons } from '../../utilities/icons';
import BasicPageContainer from '../sections/basicPageContainer';
import * as StoreNavigation from '../../store/navigation';
import { Privilege, User } from '../../api/model';
import { State } from '../../store/root';
import { connect } from 'react-redux';

interface DiplomacyPageProps {
    user : User
}

class diplomacyPage extends React.Component<DiplomacyPageProps>{
    render() {
        const gameId = (this.props as any).match.params.gameId;
        const u = this.props.user;

        const navOptions : StoreNavigation.State = {
            enableDashboard: true,
            enableLobby: true,
            enablePlay: true,
            enableSnapshots: u && u.privileges.includes(Privilege.Snapshots),
            gameId: gameId,
        };

        const i = Icons.PlayerActions;

        return (
            <BasicPageContainer>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={navOptions}/>
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
}

const mapStateToProps = (state : State) => {
    return {
        user: state.session.user
    };
}

const DiplomacyPage = connect(mapStateToProps)(diplomacyPage);
export default DiplomacyPage;