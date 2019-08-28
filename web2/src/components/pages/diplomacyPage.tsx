import * as React from 'react';
import RedirectToLoginIfNotLoggedIn from '../utilities/redirectToLoginIfNotLoggedIn';
import RedirectToLobbyIfGameNotInProgress from '../utilities/redirectToLobbyIfGameNotInProgress';
import SetNavigationOptions from '../utilities/setNavigationOptions';
import { Classes } from '../../styles/styles';
import DiplomacyPlayersTable from '../tables/diplomacyPlayersTable';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Icons from '../../utilities/icons';

export default class DiplomacyPage extends React.Component<{}>{
    render() {
        const gameId = (this.props as any).match.params.gameId;

        const navOptions = {
            enableDashboard: true,
            enableLobby: true,
            gameId: gameId,
            enablePlay: true
        };

        return (
            <div className={Classes.pageContainer}>
                <RedirectToLoginIfNotLoggedIn/>
                <RedirectToLobbyIfGameNotInProgress/>
                <SetNavigationOptions options={navOptions}/>
                <DiplomacyPlayersTable/>
                <br/>
                <div style={{width:"500px"}}>
                    <p>
                        If a player concedes (<FontAwesomeIcon icon={Icons.playerStatusConceded}/>),
                        they are removed from the turn cycle and all their pieces are abandoned. This cannot be undone.
                        If a player concedes when it is not their turn, it does not take effect until their next turn would start.
                    </p>
                    <br/>
                    <p>
                        If all players who have not conceded or been eliminated accept a draw (<FontAwesomeIcon icon={Icons.playerStatusAcceptsDraw}/>),
                        the game ends and no one wins. If you have accepted a draw, but not everyone has, you can revoke your acceptance at any time (<FontAwesomeIcon icon={Icons.revokeDraw}/>).
                    </p>
                </div>
            </div>
        );
    }
}